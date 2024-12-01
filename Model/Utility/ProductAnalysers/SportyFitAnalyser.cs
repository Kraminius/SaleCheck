
using System.Globalization;
using System.Text.RegularExpressions;
using Amazon.SecurityToken.Model;
using HtmlAgilityPack;
using SaleCheck.HtmlLib;
using Tag = SaleCheck.HtmlLib.Tag;

namespace SaleCheck.Model.Utility.ProductAnalysers 
{
    public class SportyFitAnalyser : IProductAnalyser
    {
        public async Task<List<Page>> GetRobotsSitemapLink(Page page)
        {
            string html = await page.GetHtmlContent();
            if (html == null) return new List<Page>();
            string pattern = @"Sitemap:\s*(https?://\S+|/\S+)";
            MatchCollection matches = Regex.Matches(html, pattern);
            List<Page> sitemapPages = new List<Page>();

            foreach (Match match in matches)
            {
                string link = match.Groups[1].Value;
                sitemapPages.Add(new Page("sportyfit", link));
                Console.WriteLine("HTTP MATCH: " + link);
            }

            return sitemapPages;
        }
        
        public async Task<List<Page>> GetSitemapLinks(Page page)
        {
            Console.WriteLine("Sitemap URL (top) added: " + page.GetUrl());
            string? content = await page.GetHtmlContent();
            if (content == null) return new List<Page>();
            int start = content.IndexOf("xsl\"?");
            int end = content.IndexOf("<!--");
            string contentSub = content.Substring(start + 5, end-start + 4);
            string pattern = @">https://[^\s<]+<";
            MatchCollection matches = Regex.Matches(content, pattern);
            
            Console.WriteLine(contentSub);
            Html html = new Html(contentSub);
            

            List<Page> pages = new List<Page>();
            
            foreach (Match parent in matches)
            {
                string? link = parent
                Console.WriteLine("Link: " + link);
                if(link == null) continue;
                
                Page sitemapPage = new Page("sportyfit", link);
                Console.WriteLine("SitemapPage contains: " + sitemapPage.GetUrl());
                if (link.Contains(".xml"))
                {
                    List<Page> deeperSitemaps = await GetSitemapLinks(sitemapPage);
                    pages.AddRange(deeperSitemaps);
                }
                else
                {
                    pages.Add(sitemapPage);
                    Console.WriteLine("Added sitemap: " + sitemapPage.GetUrl());
                }

                
            }

            return pages;
        }
        private static decimal ParsePrice(string? priceText)
        {
            if(priceText == null) return -1;
            string result = Regex.Replace(priceText, @"[^0-9,\.]", ""); //Remove unnecessary characters
            while(result[0] == ',' || result[0] == '.') result = result.Remove(0, 1);
            while(result[^1] == ',' || result[^1] == '.') result = result.Remove(result.Length-1, 1);
            return decimal.Parse(result);
        }
        public async Task<List<ProductItem>> Analyze(Page page)
        {
            string? content = await page.GetHtmlContent();
            if (content == null) return new List<ProductItem>();
            Html html = new Html(content);
            List<Tag> parents = html.SearchForTags(new Filter().Tag("div").Property("data-elementor-type", "loop-item"));
            List<ProductItem> productItems = new List<ProductItem>();
            foreach (Tag parent in parents)
            {
                string? id = html.SearchForTag(new Filter().Tag("div").Property("class", "yith-wcwl-add-to-wishlist"), parent)?.GetProperty("product_id");
                string? url = html.SearchForTag(new Filter().Tag("a").Property("class", "elementor-element").Property("data-element_type", "container"), parent)?.GetProperty("href");
                string? name = html.SearchForTag(new Filter().Tag("p").Property("class", "elementor-heading-title"), parent)?.ChildTags[0].Content?.Trim();
                //<p class="price product-page-price price-on-sale price-not-in-stock">
                 
                
                decimal price = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "screen-reader-text").Content("Original price"), parent)?.Content?.Trim());
                decimal discount = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "screen-reader-text").Content("Current price"), parent)?.Content?.Trim());
                decimal noDiscount = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "woocommerce-Price-amount"), parent)?.ChildTags[0]?.Content?.Trim());
                
                if (id == null || url == null || name == null) continue;
                if (price == -1 || discount == -1)
                {
                    productItems.Add(new ProductItem(url, name, id, noDiscount));
                }
                else if(discount == -1) productItems.Add(new ProductItem(url, name, id, price));
                else productItems.Add(new ProductItem(url, name, id, price, discount));
            }
            return productItems;
        }

        
    }
    
    
}
