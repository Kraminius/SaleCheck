using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SaleCheck.Model.DataClasses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaleCheck.Model.DataClasses
{
    public class Product
    {
        [BsonId]
        public string? ProductId { get; set; }

        [BsonElement("ProductName")]
        [BsonIgnoreIfNull]
        public string? ProductName { get; set; }

        [BsonElement("Price")]
        [Required]
        public List<Price> Price { get; set; } = new List<Price>();
        
        public static bool IsInViolation(List<Price> priceHistory)
        {
            var sales = priceHistory.Where(p => p.DiscountPrice > 0).OrderBy(p => p.Date).ToList();
            var nonSales = priceHistory.Where(p => p.DiscountPrice == 0).OrderBy(p => p.Date).ToList();
            
            var currentSale = sales.LastOrDefault();
            if (currentSale != null)
            {
                var saleDuration = (DateTime.UtcNow - currentSale.Date).TotalDays;
                if (saleDuration > 14)
                {
                    return true;
                }
            }
            
            var lastNonSale = nonSales.LastOrDefault();
            if (lastNonSale != null && currentSale != null)
            {
                var offSaleDuration = (currentSale.Date - lastNonSale.Date).TotalDays;
                if (offSaleDuration < 30)
                {
                    return true; // Violation: product went back on sale too soon.
                }
            }

            return false;
        }
        
        public bool CanCheckForViolations(DateTime earliestScrapeDate)
        {
            return (DateTime.UtcNow - earliestScrapeDate).TotalDays > 14;
        }



    }
    
    

}
