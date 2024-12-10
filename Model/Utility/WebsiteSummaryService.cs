﻿using SaleCheck.Model.DataClasses;
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
        var productsOnSale = website.Products.Count(p => p.SaleStreak > 0);

        // Get the earliest entry date across all products to check how long we've scraped the site
        var earliestDate = DateTime.Today;
        try
        {
            earliestDate = _productAnalysisService.GetEarliestEntryDate(website.Products);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine("Can't get earliest date, using default todays date.");
        }

        // Determine if we have scraped long enough to check for violations
        var canCheckViolations = _productAnalysisService.CanCheckForViolations(earliestDate);

        // Count the number of products violating the marketing law
        var productsViolation = canCheckViolations
            ? website.Products.Count(p => _productAnalysisService.IsProductInViolation(p))
            : 0;

        Console.WriteLine($"Number of products in violation: {productsViolation}");
        
        // Calculate the longest sale streak across all products
        var longestStreak = 0;
        try
        {
            longestStreak = _productAnalysisService.CalculateLongestSaleStreak(website.Products.ToList());
        }
        catch (Exception e)
        {
            Console.WriteLine("Not able to get longest streak. Using 0 as default value");
        }
        

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
