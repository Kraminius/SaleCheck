
using SaleCheck.Repositories.Interfaces;
using SaleCheck.Model.DataClasses;

namespace SaleCheck.Model.Utility;


public class DataFactory(IWebsiteRepository websiteRepository)
{
    public async Task UpdateWebsiteByCheckingExistingSubsites(string websiteId, bool shouldIgnoreSites)
    {
        Website website = await websiteRepository.GetWebsiteByIdAsync(websiteId);
        Dictionary<string, ProductItem> productItems = new Dictionary<string, ProductItem>();
        foreach (Subsite subsite in website.Subsites)
        {
            if(subsite.Ignore && shouldIgnoreSites) continue;
            Page page = new Page(website.WebsiteName, subsite.Url);
            subsite.Html.Add(DateTime.Now, await page.GetHtmlContent() ?? "null");
            foreach (ProductItem productItem in await page.GetProducts())
            {
                productItems.TryAdd(productItem.Name, productItem);
            }
        }
        foreach(var productKeyValue in productItems)
        {
            ProductItem productItem = productKeyValue.Value;
            if(DataConvertions.UpdateExistingProduct(website, productItem)) continue;
            website.Products.Add(DataConvertions.ProductFromProductItem(productKeyValue.Value));
        }
        await websiteRepository.UpdateWebsiteAsync(websiteId, website);
    }
    public async Task UpdateWebsiteByCheckingAll(string websiteId)
    {
        Website website = await websiteRepository.GetWebsiteByIdAsync(websiteId);
        Robots robots = new Robots(website.WebsiteName, website.WebsiteUrl);
        List<Page> pages = await  robots.GetAllSitemapPages();
        foreach (Page page in pages)
        {
            bool found = false;
            foreach (Subsite existingSubsite in website.Subsites)
            {
                if (existingSubsite.Url == page.GetUrl())
                {
                    existingSubsite.Html.Add(DateTime.Now, await page.GetHtmlContent()?? "null");
                    found = true;
                    break;
                }
            }
            if(found) continue;
            Subsite subsite = await DataConvertions.SubsiteFromPage(page);
            website.Subsites.Add(subsite);
        }
        Dictionary<string, ProductItem> productItems = await robots.GetAllProducts();
        foreach(var productKeyValue in productItems)
        {
            ProductItem productItem = productKeyValue.Value;
            if(DataConvertions.UpdateExistingProduct(website, productItem)) continue;
            website.Products.Add(DataConvertions.ProductFromProductItem(productKeyValue.Value));
        }
        await websiteRepository.UpdateWebsiteAsync(websiteId, website);
    }
    public async Task CreateWebsite(string websiteName, string websiteUrl, string websiteId)
    {
        Website website = new Website
        {
            WebsiteName = websiteName,
            WebsiteUrl = websiteUrl,
            WebsiteId = websiteId
        };
        Robots robots = new Robots(websiteName, websiteUrl);
        List<Page> pages = await  robots.GetAllSitemapPages();
        List<Subsite> subsites = new List<Subsite>();
        foreach (Page page in pages)
        {
            Subsite subsite = await DataConvertions.SubsiteFromPage(page);
            subsites.Add(subsite);
        }
        website.Subsites = subsites;
        Dictionary<string, ProductItem> productItems = await robots.GetAllProducts();
        List<Product> products = new List<Product>();
        foreach(var productKeyValue in productItems)
        {
            products.Add(DataConvertions.ProductFromProductItem(productKeyValue.Value));
        }
        website.Products = products;
        await websiteRepository.CreateWebsiteAsync(website);
    }
}