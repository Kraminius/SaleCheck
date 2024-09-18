using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using SaleCheck.Model;

namespace SaleCheck.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AnalyseController : ControllerBase
    {
        [HttpPost("HTML")]
        public ActionResult<int> DetectSaleFromHTML([FromBody] string htmlFile)
        {
            if (string.IsNullOrEmpty(htmlFile))
            {
                // Return a 400 Bad Request if the input is null or empty
                return BadRequest("Invalid HTML content.");
            }

            var products = ExtractProductsOnSale(htmlFile);

            foreach (var product in products)
            {
                Console.WriteLine($"ID: {product.ProductId}, Name: {product.ProductName}, Old Price: {product.OldPrice}, New Price: {product.NewPrice}");
            }

            return Ok();
        }

        //The basics of basic logic. Replace with proper logic. Just here to open the endpoint to test that.
        private int DetectSalePercentage(string htmlContent)
        {
            if (htmlContent.Contains("50% off", StringComparison.OrdinalIgnoreCase))
            {
                return 50;
            }
            
            else if (htmlContent.Contains("Sale", StringComparison.OrdinalIgnoreCase))
            {
                return 10;
            }
            return 0;
        }



        static List<Product> ExtractProductsOnSale(string htmlContent)
        {
            var productsOnSale = new List<Product>();

            // Load the HTML document
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var oldPriceNodes = document.DocumentNode.SelectNodes("//span[contains(@style, 'line-through') or contains(@class, 'old-price-class')]");

            if (oldPriceNodes != null)
            {
                foreach (var oldPriceNode in oldPriceNodes)
                {
                    // Navigate to the parent element which represents the product
                    var productNode = oldPriceNode.ParentNode;

                    //Extract product info
                    var productId = productNode.GetAttributeValue("id", "unknown");
                    var productNameNode = productNode.SelectSingleNode(".//h2");
                    var newPriceNode = productNode.SelectSingleNode(".//span[@class='new-price']");

                    string productName = productNameNode?.InnerText.Trim() ?? "Unnamed Product";
                    string oldPrice = oldPriceNode.InnerText.Trim();
                    string newPrice = newPriceNode?.InnerText.Trim() ?? "0";

                    productsOnSale.Add(new Product
                    {
                        ProductId = productId,
                        ProductName = productName,
                        OldPrice = oldPrice,
                        NewPrice = newPrice
                    });
                }
            }

            return productsOnSale;
        }
    }
}