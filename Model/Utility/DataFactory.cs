
using System.Runtime.InteropServices.JavaScript;
using SaleCheck.Repositories.Interfaces;
using SaleCheck.Model.DataClasses;

namespace SaleCheck.Model.Utility;


public class DataFactory(IWebsiteRepository websiteRepository)
{
    public async Task UpdateWebsiteByCheckingExistingSubsites(string websiteId, bool shouldIgnoreSites)
    {
        Website website = await websiteRepository.GetWebsiteByIdAsync(websiteId);
        Dictionary<string, ProductItem> productItems = new Dictionary<string, ProductItem>();
        
        //If subsites == empty
        if (!(website.Subsites?.Any() == true))
        {
            Console.WriteLine("No Subsites found for + " + website.WebsiteName + ", running full check");
            await UpdateWebsiteByCheckingAll(websiteId);
            return;
        }

        foreach (Subsite subsite in website.Subsites)
        {
            if(subsite.Ignore && shouldIgnoreSites) continue;
            Page page = new Page(website.WebsiteName, subsite.Url);
            subsite.Html.Add(DateTime.Now.ToString("dd-MM-yyyy") + "debug", page.GetUrl());
            foreach (ProductItem productItem in await page.GetProducts())
            {
                productItems.TryAdd(productItem.Name, productItem);
                Console.WriteLine(productItem.Name + " - " + productItem.Price);
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
                    existingSubsite.Html.Add(DateTime.Now.ToString("dd-MM-yyyy"), page.GetUrl());
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
    public async Task CreateWebsite(WebsiteInit websiteInit)
    {
        string name = websiteInit.websiteName;
        string url = websiteInit.websiteUrl;
        string id = websiteInit.websiteId;
        
        
        
        
        Website website = new Website
        {
            WebsiteName = name,
            WebsiteUrl = url,
            WebsiteId = id
        };
        Console.WriteLine(website.WebsiteName + " object created. URL: " + website.WebsiteUrl + "Website Name: " +  website.WebsiteName + "Website Id: " + website.WebsiteId);
        Robots robots = new Robots(name, url);
        List<Page> pages = await  robots.GetAllSitemapPages();
        List<Subsite> subsites = new List<Subsite>();
        await Parallel.ForEachAsync(pages, new ParallelOptions { MaxDegreeOfParallelism = 24 }, async (page, cancellationToken) =>
        {
            Subsite subsite = await DataConvertions.SubsiteFromPage(page);
            lock (subsites) 
            {
                subsites.Add(subsite);
            }
            Console.WriteLine("Subsite: " + subsite.Url + " Added");
        });
        
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