using System;
using System.Collections.Generic;
using System.Linq;
using SaleCheck.Model.DataClasses;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility
{
    public class ProductAnalysisService
    {
        public bool CanCheckForViolations(DateTime earliestScrapeDate)
        {
            Console.WriteLine($"Days scraped: {(DateTime.UtcNow - earliestScrapeDate).TotalDays}");
            return (DateTime.UtcNow - earliestScrapeDate).TotalDays > 5;
        }

        // Check if a product is in violation of the marketing rule.
        public bool IsProductInViolation(Product product)
        {
            // More than 14 consecutive days on sale is a violation
            if (product.SaleStreak > 14)
            {
                return true;
            }

            // If starting a new sale streak (SaleStreak == 1) but haven't had at least 30 no-sale days
            // since last sale, that's a violation.
            // This logic applies when a new sale period begins. If we just updated today's price and set SaleStreak to 1,
            // that means we're at the start of a new sale period.
            if (product.SaleStreak == 1 && product.NoSaleStreak > 0 && product.NoSaleStreak < 30)
            {
                return true;
            }

            return false;
        }

        public void UpdateStreaksForToday(Product product, Price todaysPrice)
        {
            bool onSaleToday = todaysPrice.DiscountPrice > 0 && (todaysPrice.DiscountPrice < todaysPrice.NormalPrice);

            if (onSaleToday)
            {
                // If we are starting a new sale period (SaleStreak == 0), check if we had enough no-sale days before this.
                if (product.SaleStreak == 0 && product.LastSaleDate.HasValue && product.NoSaleStreak < 30)
                {
                    // Not enough cooldown. This is a violation scenario, but we still update the state.
                    product.SaleStreak = 1;
                    product.NoSaleStreak = 0;
                    product.LastSaleDate = todaysPrice.Date;
                }
                else
                {
                    // Continue the sale streak or start a new one properly
                    if (product.SaleStreak == 0)
                    {
                        product.SaleStreak = 1; // New sale streak starts
                    }
                    else
                    {
                        product.SaleStreak++;
                    }

                    product.NoSaleStreak = 0;
                    product.LastSaleDate = todaysPrice.Date;
                }
            }
            else
            {
                // Not on sale today, increment no sale streak
                product.NoSaleStreak++;

                // Once NoSaleStreak >= 29, we consider that the sale streak can be reset
                // because we have had a full 30-day cooldown since the last sale.
                if (product.NoSaleStreak > 29)
                {
                    product.SaleStreak = 0;
                }
            }
            
        }

        public int CalculateLongestSaleStreak(List<Product> products)
        {
            int longestStreak = 0;
            foreach (var product in products)
            {
                if (product.SaleStreak > longestStreak)
                    longestStreak = product.SaleStreak;
            }
            return longestStreak;
        }

        public DateTime GetEarliestEntryDate(IEnumerable<Product> products)
        {
            // Find the earliest price entry date across all products
            return products
                .SelectMany(p => p.Price)
                .Min(p => p.Date);
        }
    }
}
