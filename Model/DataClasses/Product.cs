using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
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

        [BsonElement("SaleStreak")]
        public int SaleStreak { get; set; }

        [BsonElement("NoSaleStreak")]
        public int NoSaleStreak { get; set; }

        [BsonElement("LastSaleDate")]
        public DateTime? LastSaleDate { get; set; }
    }
}