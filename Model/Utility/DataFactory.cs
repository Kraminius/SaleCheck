using System.Collections.ObjectModel;
using SaleCheck.Model.DataClasses;
using SaleCheck.Repositories.Interfaces;

namespace SaleCheck.Model.Utility;

public class DataFactory(IWebsiteRepository websiteRepository)
{
    private readonly IWebsiteRepository _websiteRepository = websiteRepository;

    public async Task CreateWebsite(string websiteName, string websiteUrl, string websiteID)
    {
        Website website = new Website();
        website.WebsiteName = websiteName;
        website.WebsiteUrl = websiteUrl;
        website.WebsiteId = websiteID;
        Robots robots = new Robots(websiteName, websiteUrl);
        List<Page> pages = await  robots.GetAllSitemapPages();
        List<Subsite> subsites = new List<Subsite>();
        foreach (Page page in pages)
        {
            Subsite subsite = new Subsite();
            subsite.Url = page.GetUrl();
            Dictionary<DateTime, string> instanceOfPages = new Dictionary<DateTime, string>();
            instanceOfPages.Add(DateTime.Now, page.GetUrl());
            subsite.Html = instanceOfPages;
            subsite.Ignore = false;
        }
        website.Subsites = subsites;
        Dictionary<string, ProductItem> productItems = await robots.GetAllProducts();
        List<Product> products = new List<Product>();
        foreach(var productKeyValue in productItems)
        {
            Product product = new Product();
            ProductItem productItem = productKeyValue.Value;
            product.ProductName = productItem.Name;
            product.ProductId = productItem.Id;
            List<Price> prices = new List<Price>();
            Price price = new Price();
            price.Date = DateTime.Now;
            if (productItem.OtherPrice != null)
            {
                price.DiscountPrice = Decimal.ToDouble(productItem.Price);
                decimal normalPrice = productItem.OtherPrice ?? 0;
                price.NormalPrice = Decimal.ToDouble(normalPrice);
            }
            else
            {
                price.NormalPrice = Decimal.ToDouble(productItem.Price);
            }
            prices.Add(price);
            product.Price = prices;
        }
        website.Products = products;
        await _websiteRepository.CreateWebsiteAsync(website);

    }
}