using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SaleCheck.Model.DataClasses
{
    public class Website
    {

        [BsonId]
        [BsonRepresentation(BsonType.Int32)]
        public required string WebsiteId { get; set; }

        [BsonElement("WebsiteUrl")]
        [Required(ErrorMessage = "Website URL is required.")]
        public required string WebsiteUrl { get; set; }

        [BsonElement("WebsiteName")]
        [Required(ErrorMessage = "Website name is required.")]
        public required string WebsiteName { get; set; }

        [BsonElement("Tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [BsonElement("Products")]
        public List<Product> Products { get; set; } = new List<Product>();

        [BsonElement("Subsites")]
        public List<Subsite> Subsites { get; set; } = new List<Subsite>();
    }
}
