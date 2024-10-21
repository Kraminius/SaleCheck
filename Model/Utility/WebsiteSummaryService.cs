using SaleCheck.Model.DataClasses;
using SaleCheck.Tests.SaleCheck.Tests.Model.Utility;

namespace SaleCheck.Model.Utility;

public class WebsiteSummaryService
{
    private readonly ProductAnalysisService _productAnalysisService;

    public WebsiteSummaryService(ProductAnalysisService productAnalysisService)
    {
        _productAnalysisService = productAnalysisService;
    }

    public WebsiteSummary CreateWebsiteSummary(Website website)
    {
        var productsOnSale = website.Products.Count(p => p.Price.Any(price => price.DiscountPrice > 0));

        // Get the earliest entry date across all products to check how long we've scraped the site
        var earliestDate = _productAnalysisService.GetEarliestEntryDate(website.Products);

        // Determine if we have scraped long enough to check for violations
        var canCheckViolations = _productAnalysisService.CanCheckForViolations(earliestDate);

        // Count the number of products violating the marketing law
        var productsViolation = canCheckViolations
            ? website.Products.Count(p => _productAnalysisService.IsProductInViolation(p.Price))
            : 0;
        Console.WriteLine($"Number of products in violation: {productsViolation}");


        // Calculate the longest sale streak across all products
        var longestStreak = website.Products.Max(p => _productAnalysisService.CalculateLongestSaleStreak(p.Price));

        // Return the WebsiteSummary object
        return new WebsiteSummary
        {
            WebsiteId = website.WebsiteId,
            WebsiteUrl = website.WebsiteUrl,
            WebsiteName = website.WebsiteName,
            ProductsCount = website.Products.Count,
            ProductsOnSale = productsOnSale,
            ProductsViolation = productsViolation,
            ProductsLongestCurrentStreak = longestStreak
        };
    }

}
