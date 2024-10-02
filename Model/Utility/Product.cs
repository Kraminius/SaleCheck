namespace SaleCheck.Model.Utility;

public class Product(string url, string name, string id, double price, double? otherPrice = null)
{
    public string Url = url;
    public string Name = name;
    public string Id = id;
    public double Price = price;
    public double? OtherPrice = otherPrice;

    public override string ToString()
    {
        if(OtherPrice != null) return $"{Name} : {Id}, dPrice:{Price}, nPrice:{OtherPrice}";
        return $"{Name} : {Id} : nPrice:{Price}, dPrice: none";
    }
}