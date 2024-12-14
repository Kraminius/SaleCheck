public class ProductItem(string url, string name, string id, decimal normalPrice, decimal? discountPrice = null)
{
    public string Url = url;
    public string Name = name;
    public string Id = id;
    public decimal NormalPrice = normalPrice;
    public decimal? DiscountPrice = discountPrice;

    public override string ToString()
    {
        if (DiscountPrice != null) return $"{Name} : {Id}, dPrice:{DiscountPrice}, nPrice:{NormalPrice}";
        return $"{Name} : {Id} : nPrice:{NormalPrice}, dPrice: none";
    }

    public bool Equals(ProductItem obj)
    {
        return string.Equals(Id, obj.Id, StringComparison.OrdinalIgnoreCase);
    }
}