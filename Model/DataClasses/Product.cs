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
        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "No product ID detected.")]
        public required int ProductId { get; set; }

        [BsonElement("ProductName")]
        [BsonIgnoreIfNull]
        public string? ProductName { get; set; }

        [BsonElement("Price")]
        [Required]
        public List<Price> Price { get; set; } = new List<Price>();

        // Old price and new price only present for ease of testing.
        public required string OldPrice { get; set; }
        public required string NewPrice { get; set; }
    }
}
