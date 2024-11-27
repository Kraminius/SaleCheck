using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaleCheck.Model.DataClasses
{
    public class ScraperWebsiteRules
    {
        public required string WebsiteName { get; set; }
        public required string WebsiteUrl { get; set; }
        public required string ExampleHtml { get; set; }
        public required Dictionary<string, string[]> Commands { get; set; }
    }
        
}