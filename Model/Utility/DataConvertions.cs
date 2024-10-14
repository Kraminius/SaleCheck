﻿using SaleCheck.Model.DataClasses;

namespace SaleCheck.Model.Utility;

public class DataConvertions
{
    
    public static bool UpdateExistingProduct(Website website, ProductItem productItem)
    {
        foreach (Product existingProduct in website.Products)
        {
            if (existingProduct.ProductId == productItem.Id && existingProduct.ProductName == productItem.Name)
            {
                Price newPrice = PriceFromProductItem(productItem);
                existingProduct.Price.Add(newPrice);
                return true;
            }
        }
        return false;
    }
    public static async Task<Subsite> SubsiteFromPage(Page page)
    {
        Dictionary<DateTime, string> instanceOfPages = new Dictionary<DateTime, string>();
        instanceOfPages.Add(DateTime.Now, await page.GetHtmlContent() ?? "null");
        return new Subsite
        {
            Url = page.GetUrl(),
            Html = instanceOfPages,
            Ignore = (await page.GetProducts()).Count == 0          //No products were found
        };
    }
    public static Product ProductFromProductItem(ProductItem item)
    {
        List<Price> prices = new List<Price>();
        Price price = PriceFromProductItem(item);
        prices.Add(price);
        Product product = new Product
        {
            ProductName = item.Name,
            ProductId = item.Id,
            Price = prices
        };
        return product;
    }
    public static Price PriceFromProductItem(ProductItem item)
    {
        
        if (item.OtherPrice != null)
        {
            decimal normalPrice = item.OtherPrice ?? 0;
            return new Price
            {
                Date = DateTime.Now,
                DiscountPrice = Decimal.ToDouble(item.Price),
                NormalPrice = Decimal.ToDouble(normalPrice)
            };
        }
        else
        {
            return new Price
            {
                Date = DateTime.Now,
                NormalPrice = Decimal.ToDouble(item.Price)
            };
        }
    }
}