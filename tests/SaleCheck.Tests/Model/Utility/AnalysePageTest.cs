using System;
using System.IO;
using SaleCheck.Model.Utility;
using Xunit;
using Xunit.Abstractions;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility
{
    public class AnalysePageTest
    {
        /*
        private readonly ITestOutputHelper _output;

        public AnalysePageTest(ITestOutputHelper output)
        {
            _output = output;
        }
        /*
        [Fact]
        public void Analyze_FromFilePath_ReturnsProducts()
        {

            string fileName = "vinduesgrossisten.html";
            string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            string filePath = Path.Combine(projectDirectory,"SaleCheck","tests", "SaleCheck.Tests", "Model", "Utility", "TestData", fileName);

            string htmlContent = AnalysePage.ReadHtmlFile(filePath);

            Assert.False(string.IsNullOrEmpty(htmlContent), "Failed to read the HTML file.");

            var products = AnalysePage.Analyze(htmlContent);

            Assert.NotNull(products);
            Assert.NotEmpty(products);

            foreach (var product in products)
            {
                Assert.False(string.IsNullOrEmpty(product.ProductId), "Product ID should not be null or empty.");
                Assert.False(string.IsNullOrEmpty(product.Name), "Product Name should not be null or empty.");
                Assert.True(product.Price > 0, "Product Price should be greater than zero.");
                _output.WriteLine("-------------------------------------");
                _output.WriteLine($"Product ID: {product.ProductId}");
                _output.WriteLine($"Product Name: {product.Name}");
                _output.WriteLine($"Description: {product.AdditionalDescription}");
                _output.WriteLine($"Price: {product.Price:C}");
                if (product.DiscountPrice.HasValue)
                {
                    _output.WriteLine($"Discount Price: {product.DiscountPrice.Value:C}");
                }
               

                if (product.DiscountPrice.HasValue)
                {
                    Console.WriteLine($"Discount Price: {product.DiscountPrice.Value:C}");
                }
            }
        }
    */
    }
}
