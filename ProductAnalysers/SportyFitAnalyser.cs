using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using SaleCheck.HtmlLib;
using Tag = SaleCheck.HtmlLib.Tag;

namespace SaleCheck.Model.Utility.ProductAnalysers
{
    public class SportyFitAnalyser : IProductAnalyser
    {
        // Hard-coded list of category URLs. See report for reasoning.
        private static readonly List<string> HardCodedSitemaps = new List<string>
        {
            "https://sportyfit.dk/product_cat-sitemap1.xml",
            "https://sportyfit.dk/product_cat-sitemap2.xml"
        };

        public async Task<List<Page>> GetRobotsSitemapLink(Page page)
        {
            return HardCodedSitemaps.Select(url => new Page(SampleSites.SportyFit.Title, url)).ToList();
        }

        public async Task<List<Page>> GetSitemapLinks(Page page)
        {
            string? html = await page.GetHtmlContent();
            if (html == null) return new List<Page>();

            string pattern = @">https://[^\s<]+<";
            MatchCollection matches = Regex.Matches(html, pattern);
            List<Page> pages = new List<Page>();

            foreach (Match match in matches)
            {
                string link = match.Value.Trim('>', '<');
                pages.Add(new Page(SampleSites.SportyFit.Title, link));
            }

            return pages.Distinct().ToList();
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
            
            Console.WriteLine("Analyzing products of SportyFit...");
            
            string? content = await page.GetHtmlContent();
            if (content == null) return new List<ProductItem>();

            Html html = new Html(content);
            List<Tag> parents = html.SearchForTags(new Filter().Tag("div").Property("class", "e-loop-item"));
            List<ProductItem> productItems = new List<ProductItem>();

            foreach (Tag parent in parents)
            {
                string? url = html.SearchForTag(new Filter().Tag("a").Property("class", "elementor-element").Property("data-element_type", "container"), parent)?.GetProperty("href");
                string? name = html.SearchForTag(new Filter().Tag("p").Property("class", "elementor-heading-title"), parent)?.ChildTags[0].Content?.Trim();
                string? id = name;

                decimal price = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "screen-reader-text").Content("Original price"), parent)?.Content?.Trim());
                decimal discount = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "screen-reader-text").Content("Current price"), parent)?.Content?.Trim());
                decimal noDiscount = ParsePrice(html.SearchForTag(new Filter().Tag("span").Property("class", "woocommerce-Price-amount"), parent)?.ChildTags[0]?.Content?.Trim());

                if (id == null || url == null || name == null)
                {
                    productItems.Add(new ProductItem(url ?? "url is null", name ?? "name is null", id ?? "id is null", 0));
                }
                else if (price == -1 || discount == -1)
                {
                    productItems.Add(new ProductItem(url, name, id, noDiscount));
                }
                
                else if (discount == -1) productItems.Add(new ProductItem(url, name, id, price));
                else
                {
                    productItems.Add(discount > price
                        ? new ProductItem(url, name, id, discount, price)
                        : new ProductItem(url, name, id, price, discount));
                }
            }

            return productItems;
        }
        

    }
}
