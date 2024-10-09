using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
        
        public async Task<List<Product>> Analyze(Page page)
        {
            List<Product> products = new List<Product>();
            var htmlDoc = new HtmlDocument();
            string? content = await page.GetHtmlContent();
            if (content == null) return new List<Product>();
            htmlDoc.LoadHtml(content);
            
            var productNodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='product-info-link']");

            if (productNodes == null)
            {
                return products; 
            }

            foreach (var productNode in productNodes)
            {
                try
                {
                    // Extract Product ID
                    var priceBoxNode = productNode.SelectSingleNode(".//div[contains(@class, 'price-box')]");
                    string productId = priceBoxNode?.GetAttributeValue("data-product-id", "N/A") ?? "N/A";

                    // Extract Product Name
                    var nameNode = productNode.SelectSingleNode(".//h3[@class='name']");
                    string name = nameNode?.InnerText.Trim() ?? "N/A";

                    // Extract Additional Description
                    var descriptionNode = productNode.SelectSingleNode(".//p[@class='additional-description']");
                    name += " " + descriptionNode?.InnerText.Trim() ?? "";
                    
                    // Extract Prices
                    var priceNode = productNode.SelectSingleNode(".//div[@class='price']");
                    var priceText = priceNode?.InnerText.Trim() ?? "N/A";
                    decimal price = ParsePrice(priceText);
                    decimal otherPrice = -1;
                    var discountPriceNode = productNode.SelectSingleNode(".//div[contains(@class, 'price_oprice_with_discountld')]");
                    if (discountPriceNode != null)
                    {
                        var discountPriceText = discountPriceNode.InnerText.Trim();
                        otherPrice = ParsePrice(discountPriceText);
                    }
                    
                    Product product = (otherPrice != -1) 
                        ? new Product(page.GetUrl(), name, productId, price, otherPrice) 
                        : new Product(page.GetUrl(), name, productId, price);
                    products.Add(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while processing a product: {ex.Message}");
                }
            }

            return products;
        }

        private static decimal ParsePrice(string priceText)
        {
            var cleanedPrice = priceText.Replace("kr.", "").Replace(" ", "").Trim();
            var normalizedPrice = cleanedPrice.Replace(".", "").Replace(",", ".");

            if (decimal.TryParse(normalizedPrice, NumberStyles.Number, CultureInfo.InvariantCulture,
                    out decimal priceValue))
            {
                return priceValue;
            }
            else
            {
                throw new FormatException($"Unable to parse price: {priceText}");
            }
        }
    }
    
    
}
