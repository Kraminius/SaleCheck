using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SaleCheck.Model.DataClasses
{
    public class Subsite
    {
        [BsonElement("Url")]
        [Required(ErrorMessage = "No URL detected.")]
        public string Url { get; set; }

        [BsonElement("Ignore")]
        public bool Ignore { get; set; }

        [BsonElement("Html")]
        public Dictionary<string, string> Html { get; set; } = new Dictionary<string, string>();


    }
}
