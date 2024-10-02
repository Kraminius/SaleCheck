using System;
using System.Collections.Generic;
using System.Globalization;
using HtmlAgilityPack;

namespace SaleCheck.Model.Utility 
{
    public class Product
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string AdditionalDescription { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
    }

    public class AnalysePage
    {
        public List<Product> Analyze(string htmlContent)
        {
            var products = new List<Product>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var productNodes = htmlDoc.DocumentNode.SelectNodes("//a[@class='product-info-link']");

            if (productNodes == null)
            {
                return products; 
            }

            foreach (var productNode in productNodes)
            {
                try
                {
                    var product = new Product();

                    // Extract Product ID
                    var priceBoxNode = productNode.SelectSingleNode(".//div[contains(@class, 'price-box')]");
                    product.ProductId = priceBoxNode?.GetAttributeValue("data-product-id", "N/A") ?? "N/A";

                    // Extract Product Name
                    var nameNode = productNode.SelectSingleNode(".//h3[@class='name']");
                    product.Name = nameNode?.InnerText.Trim() ?? "N/A";

                    // Extract Additional Description
                    var descriptionNode = productNode.SelectSingleNode(".//p[@class='additional-description']");
                    product.AdditionalDescription = descriptionNode?.InnerText.Trim() ?? "N/A";

                    // Extract Prices
                    var priceNode = productNode.SelectSingleNode(".//div[@class='price']");
                    var priceText = priceNode?.InnerText.Trim() ?? "N/A";
                    product.Price = ParsePrice(priceText);

                    var discountPriceNode = productNode.SelectSingleNode(".//div[contains(@class, 'price_oprice_with_discountld')]");
                    if (discountPriceNode != null)
                    {
                        var discountPriceText = discountPriceNode.InnerText.Trim();
                        product.DiscountPrice = ParsePrice(discountPriceText);
                    }

                    products.Add(product);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while processing a product: {ex.Message}");
                }
            }

            return products;
        }

        private decimal ParsePrice(string priceText)
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

        public string ReadHtmlFile(string filePath)
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
