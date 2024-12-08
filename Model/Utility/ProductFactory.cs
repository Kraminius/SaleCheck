using System.Text.RegularExpressions;
using SaleCheck.HtmlLib;

namespace SaleCheck.Model.Utility;

public class ProductFactory
{
    public class Filters(
        Filter productParent,
        Filter productId,
        Filter productName,
        Filter productLink,
        Filter productPrice,
        Filter productNormalPrice,
        Filter productDiscountPrice)
    {
        public readonly Filter ProductParent = productParent;
        public readonly Filter ProductId = productId;
        public readonly Filter ProductName = productName;
        public readonly Filter ProductLink = productLink;
        public readonly Filter ProductPrice = productPrice;
        public readonly Filter ProductNormalPrice = productNormalPrice;
        public readonly Filter ProductDiscountPrice = productDiscountPrice;
    }

    public static List<ProductItem> GetProducts(string stringHtml, Filters filters)
    {
        Html html = new Html(stringHtml);
        List<Tag> tags = html.SearchForTags(filters.ProductParent);
        List<ProductItem> products = new List<ProductItem>();
        foreach (Tag tag in tags) products.Add(GetProduct(html, tag, filters)!);
        return products;
    }
    private static ProductItem? GetProduct(Html html, Tag parent, Filters rules)
    {
        string? id = html.SearchForValue(rules.ProductId, parent);
        string? link = html.SearchForValue(rules.ProductLink, parent);
        string? name = html.SearchForValue(rules.ProductName, parent);
        string? price = html.SearchForValue(rules.ProductNormalPrice, parent);
        string? discount = html.SearchForValue(rules.ProductDiscountPrice, parent);
        string? noDiscount = html.SearchForValue(rules.ProductPrice, parent);
        if (name == null || link == null || id == null) return null;
        if(discount == null && price != null) return new ProductItem(link, id, name, ToDecimal(price));
        else if(discount != null && price != null) return new ProductItem(link, id, name, ToDecimal(price), ToDecimal(discount));
        else if(noDiscount != null) return new ProductItem(link, id, name, ToDecimal(noDiscount));
        return null;
    }

    private static decimal ToDecimal(string price)
    {
        string result = Regex.Replace(price, @"[^0-9,\.]", ""); //Remove unnecessary characters
        return decimal.Parse(result);
    }
    
}