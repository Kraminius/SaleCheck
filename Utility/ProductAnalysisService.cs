using SaleCheck.Model.DataClasses;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;

public class ProductAnalysisService
{
    public bool CanCheckForViolations(DateTime earliestScrapeDate)
    {
        Console.WriteLine($"Days scraped: {(DateTime.UtcNow - earliestScrapeDate).TotalDays}");
        return (DateTime.UtcNow - earliestScrapeDate).TotalDays > 5;
    }

    public bool IsProductInViolation(List<Price> priceHistory)
    {
        var sortedPrices = priceHistory.OrderBy(p => p.Date).ToList();

        int saleStreak = 0;
        DateTime? lastDiscountDate = null;

        foreach (var price in sortedPrices)
        {
            if (price.DiscountPrice > 0 && (price.DiscountPrice < price.NormalPrice))
            {
                // The product is on sale, increase the streak
                saleStreak++;

                lastDiscountDate = price.Date;

                if (saleStreak > 14)
                {
                    return true;
                }
            }
            else
            {
                if (lastDiscountDate.HasValue && (price.Date - lastDiscountDate.Value).TotalDays >= 30)
                {
                    saleStreak = 0;
                }

                lastDiscountDate = null;
            }
        }

        return false;
    }

    
    public int CalculateLongestSaleStreak(List<Price> priceHistory)
    {
        var sortedPrices = priceHistory.OrderBy(price => price.Date.Date).ToList();  // Use Date only, ignore time
        int saleStreak = 0;
        int maxStreak = 0;
        DateTime? lastDiscountDate = null;

        foreach (var price in sortedPrices)
        {
            if (price.DiscountPrice > 0 && price.DiscountPrice < price.NormalPrice)
            {
                
                if (lastDiscountDate.HasValue && (price.Date.Date - lastDiscountDate.Value.Date).TotalDays == 1)
                {
                    saleStreak++;  // Consecutive sale day
                }
                else
                {
                    saleStreak = 1;  // Start new streak if it's the first day or not consecutive
                }

                lastDiscountDate = price.Date;
                
                if (saleStreak > maxStreak)
                {
                    maxStreak = saleStreak;
                }
            }
            else
            {
                // Reset the sale streak if there has been 30 days of no sale
                if (lastDiscountDate.HasValue && (price.Date - lastDiscountDate.Value).TotalDays >= 30)
                {
                    saleStreak = 0;
                }
                lastDiscountDate = null;
            }
        }

        return maxStreak;
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
