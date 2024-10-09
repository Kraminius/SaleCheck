namespace SaleCheck.Model.Utility;

public class ProductItem(string url, string name, string id, decimal price, decimal? otherPrice = null)
{
    public string Url = url;
    public string Name = name;
    public string Id = id;
    public decimal Price = price;
    public decimal? OtherPrice = otherPrice;

    public override string ToString()
    {
        if(OtherPrice != null) return $"{Name} : {Id}, dPrice:{Price}, nPrice:{OtherPrice}";
        return $"{Name} : {Id} : nPrice:{Price}, dPrice: none";
    }
    
    public bool Equals(ProductItem obj)
    {
        return string.Equals(Id, obj.Id, StringComparison.OrdinalIgnoreCase);
    }
}