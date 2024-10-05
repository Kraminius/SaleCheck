using System.Globalization;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SaleCheck.Model.Utility
{
    public class ElgigantenAnalyser : IProductAnalyser
    {
        public async Task<string[]> RegexMatchLinksFromRobots(Page page)
        {
            string html = await page.GetHtmlContent();
            if (html == null) return new string[0];
            string pattern = @"Sitemap:\s*(https?://\S+|/\S+)";
            MatchCollection matches = Regex.Matches(html, pattern);
            List<string> sitemapLinks = new List<string>();

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

                sitemapLinks.Add(link);
            }

            return sitemapLinks.ToArray();
        }
        public async Task<string[]> RegexMatchLinksFromSitemap(Page page)
        {
            string html = await page.GetHtmlContent();
            if (html == null) return new string[0];
            string pattern = @">https://[^\s<]+<";
            MatchCollection matches = Regex.Matches(html, pattern);
            List<string> links = new List<string>();

            foreach (Match match in matches)
            {
                // Remove the wrapping '>' and '<'
                string link = match.Value.Trim('>', '<');
                links.Add(link);
            }

            //There are two global sitemaps, one containing multiple sitemaps for images and another for actual sites.
            if (links.Count > 20)
                return new string[0]; //We remove the sitemap for images because it has about 22 sitemaps.

            string categoryLink = "";
            foreach (string link in links)
            {
                if (link.Contains("category")) categoryLink = link;
            }

            Page categoryPage = new Page(SampleSites.Elgiganten.Title, categoryLink);

            html = await categoryPage.GetHtmlContent();
            if (html == null) return new string[0];

            // This pattern will capture only the URLs inside <loc> tags, excluding <image:loc>
            pattern = @"<loc>(https://[^\s<]+)</loc>";

            matches = Regex.Matches(html, pattern);
            links = new List<string>();

            foreach (Match match in matches)
            {
                // The actual link is in the first capturing group
                string link = match.Groups[1].Value;
                links.Add(link);
            }

            return links.ToArray();
        }
        public async Task<List<Product>> Analyze(Page page)
        {
            List<Product> products = new List<Product>();
            var htmlDoc = new HtmlDocument();
            string? content = await page.GetHtmlContent();
            if (content == null) return new List<Product>();
            htmlDoc.LoadHtml(content);

            // Select all product cards using the data-testid attribute
            var productNodes = htmlDoc.DocumentNode.SelectNodes("//a[@data-type='product']");

            if (productNodes == null)
            {
                products.Add(new Product("", "no product nodes found", "", 0, 0));
                return products;
            }

            foreach (var productNode in productNodes)
            {
                try
                {
                    // Extract Product ID from the data-item-id attribute
                    string productId = productNode.GetAttributeValue("data-item-id", "N/A");

                    // Extract Product Name (it's in the <h2> tag within the product card)
                    var nameNode = productNode.SelectSingleNode(".//h2");
                    string name = nameNode?.InnerText.Trim() ?? "N/A";

                    // Extract Price (it's inside a span with font-headline class)
                    var priceNode = productNode.SelectSingleNode(".//span[@class='font-headline']");
                    string priceText = priceNode?.InnerText.Trim() ?? "N/A";
                    decimal price = ParsePrice(priceText);

                    // Optionally, extract any additional details or description (e.g., discount price)
                    var discountNode = productNode.SelectSingleNode(".//span[@data-highlight-price]");
                    decimal discountPrice = -1;
                    if (discountNode != null)
                    {
                        string discountText = discountNode.InnerText.Trim();
                        discountPrice = ParsePrice(discountText);
                    }

                    // Create a product object based on whether a discount price exists
                    Product product = (discountPrice != -1)
                        ? new Product(page.GetUrl(), name, productId, price, discountPrice)
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
            // Clean and normalize the price text for parsing
            var cleanedPrice = priceText.Replace(".", "").Replace(",", ".").Replace("-", "").Trim();

            if (decimal.TryParse(cleanedPrice, NumberStyles.Number, CultureInfo.InvariantCulture,
                    out decimal priceValue))
            {
                return priceValue;
            }
            else
            {
                throw new FormatException($"Unable to parse price: {priceText}");
            }
        }


        public static string ReadHtmlFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"The file at '{filePath}' does not exist.");
                    return null;
                }

                string htmlContent = File.ReadAllText(filePath);
                return htmlContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
                return null;
            }
        }
    }
}
