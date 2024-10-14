using SaleCheck.Model.DataClasses;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;

public class ProductAnalysisService
{
    public bool CanCheckForViolations(DateTime earliestScrapeDate)
    {
        return (DateTime.UtcNow - earliestScrapeDate).TotalDays > 14;
    }

    public bool IsProductInViolation(List<Price> priceHistory)
    {
        var firstPrice = priceHistory.OrderBy(p => p.Date).FirstOrDefault();

        if (firstPrice != null && firstPrice.DiscountPrice > 0)
        {
            return true; // Violation: product starts on sale.
        }

        // Check if product goes on sale before 30 days
        foreach (var price in priceHistory)
        {
            if (price.DiscountPrice > 0 && (price.Date - firstPrice.Date).TotalDays < 30)
            {
                return true;
            }
        }

        return false; 
    }
    
    public DateTime GetEarliestEntryDate(IEnumerable<Product> products)
    {
        // Find the earliest price entry date across all products -
        // we need this to check if we can determine if a product went straight on sale.
        return products
            .SelectMany(p => p.Price)
            .Min(p => p.Date);
    }
}
