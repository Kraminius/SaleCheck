using System;
using System.Collections.Generic;
using System.Globalization;
using HtmlAgilityPack;

namespace SaleCheck.Model.Utility 
{
    public static class AnalysePage
    {
        public static async Task<List<Product>> Analyze(Page page)
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

            if (decimal.TryParse(normalizedPrice, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal priceValue))
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
