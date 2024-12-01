using System.Globalization;
using System.Text.RegularExpressions;
using Amazon.SecurityToken.Model;
using HtmlAgilityPack;
using SaleCheck.HtmlLib;
using Tag = SaleCheck.HtmlLib.Tag;

namespace SaleCheck.Model.Utility.ProductAnalysers 
{
    public class VinduesGrossistenAnalyser : IProductAnalyser
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
            
                /*
                 Some robots.txt write their sitemap as /sitemap.xsl,
                 while others use the whole link as https://website.dk/sitemap.xml
                 we therefore add the base url to the front of /sitemap to make sure we get links that can be used.
                 */
                if (!link.StartsWith("http")) 
                {
                    link = page.GetUrl().TrimEnd('/') + link;
                } 

                // Skip link if it is not danish, other countries is not within our scope. (Only does something for pages who sell globally)
                if (!link.Contains(".dk") && !link.Contains("da_dk"))
                {
                    continue; 
                }

                sitemapPages.Add(new Page(SampleSites.VinduesGrossisten.Title, link));
            }

            return sitemapPages;
        }
        
        public async Task<List<Page>> GetSitemapLinks(Page page)
        {
            string html = await page.GetHtmlContent();
            if (html == null) return new List<Page>();
            string pattern = @">https://[^\s<]+<";
            MatchCollection matches = Regex.Matches(html, pattern);
            List<Page> pages = new List<Page>();

            foreach (Match match in matches)
            {
                // Remove the wrapping '>' and '<'
                string link = match.Value.Trim('>', '<');
                pages.Add(new Page(SampleSites.VinduesGrossisten.Title,link));
            }

            return pages;
        }
        private static decimal ParsePrice(string? priceText)
        {
            if (string.IsNullOrEmpty(priceText)) return -1;

            // Remove unnecessary characters, keeping only numbers, ',' and '.'
            string result = Regex.Replace(priceText, @"[^\d,\.]", "");

            // Normalize the string: assume ',' is the decimal separator and '.' is the thousand separator
            result = result.Replace(".", "").Replace(",", ".");
            
            return decimal.Parse(result);
        }

        public async Task<List<ProductItem>> Analyze(Page page)
        {
            string? content = await page.GetHtmlContent();
            if (content == null) return new List<ProductItem>();
            Html html = new Html(content);
            List<Tag> parents = html.SearchForTags(new Filter().Tag("figcaption").Property("class", "product-item-details"));
            List<ProductItem> productItems = new List<ProductItem>();
            foreach (Tag parent in parents)
            {
                string? id = html.SearchForTag(new Filter().Tag("div").Property("class", "price-box"), parent)?.GetProperty("data-product-id");
                string? url = html.SearchForTag(new Filter().Tag("a").Property("class", "product-info-link"), parent)?.GetProperty("href");
                string? name = html.SearchForTag(new Filter().Tag("h3").Property("class", "name"), parent)?.Content?.Trim();
                if (name != null)
                    name += " " + html
                        .SearchForTag(new Filter().Tag("p").Property("class", "additional-description"), parent)
                        ?.Content?.Trim();//Name was split into two containers
                decimal price = ParsePrice(html.SearchForTag(new Filter().Tag("div").Property("class", "price"), parent)?.Content?.Trim());
                decimal discount = ParsePrice(html.SearchForTag(new Filter().Tag("div").Property("class", "price_oprice_with_discountld"), parent)?.Content?.Trim());
                if (id == null || url == null || name == null) continue;
                if(discount == -1) productItems.Add(new ProductItem(url, name, id, price));
                else productItems.Add(new ProductItem(url, name, id, price, discount));
            }
            return productItems;
        }

        
    }
    
    
}
