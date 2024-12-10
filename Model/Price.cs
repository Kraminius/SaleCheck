using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace SaleCheck.Model.DataClasses
{
    public class Price
    {

        [BsonElement("Date")]
        public DateTime Date { get; set; }

        [BsonElement("NormalPrice")]
        public double NormalPrice { get; set; }

        [BsonElement("DiscountPrice")]
        [BsonIgnoreIfNull]
        public double DiscountPrice { get; set; }
    }
}
