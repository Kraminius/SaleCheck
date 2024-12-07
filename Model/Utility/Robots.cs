using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace SaleCheck.Model.Utility;

public class Robots(string title, string url)
{
    private Page? _page;
    private List<SiteMap> _siteMaps = new List<SiteMap>();
    private Dictionary<string, ProductItem> _products = new Dictionary<string, ProductItem>();
    public async Task<List<SiteMap>> GetSiteMaps()
    {
        if (_siteMaps.Count == 0) await LoadSiteMaps();
        return _siteMaps;
    }

    public async Task<bool> LoadRobots()
    {
        _page = new Page(title, url + "robots.txt");
        return (await _page.GetHtmlContent() != null);
    }
    private async Task LoadSiteMaps()
    {
        if (_page == null)
            if (!await LoadRobots())
                return;

        string? html = await _page.GetHtmlContent();
        if (html == null)
            return; // Returns if none is found

        IProductAnalyser? analyser = ProductAnalyser.GetAnalyser(title);
        if (analyser == null)
            return;

        List<Page> sitemapLinks = await analyser.GetRobotsSitemapLink(_page);
        _siteMaps = new List<SiteMap>();

        var siteMapTasks = sitemapLinks.Select(async sitemapLink =>
        {
            var siteMap = new SiteMap(title, sitemapLink.GetUrl());
            await siteMap.LoadSiteMapsAsync();
            _siteMaps.Add(siteMap);
        });

        await Task.WhenAll(siteMapTasks);
    }

    public async Task<String?> GetRobotsContent()
    {
        if (_page == null)
        {
            return null;
        } 
        return await this._page!.GetHtmlContent();
    }


    public async Task<List<Page>> GetAllSitemapPages()
    {
        List<SiteMap> sitemaps = await GetSiteMaps();
        var allPages = new ConcurrentBag<Page>();

        var pageTasks = sitemaps.Select(async siteMap =>
        {
            await siteMap.LoadSiteMapsAsync();
            List<Page> siteMapPages = siteMap.GetPages();
            foreach (var page in siteMapPages)
            {
                allPages.Add(page);
            }
        });

        await Task.WhenAll(pageTasks);

        return allPages.ToList();
    }


    public async Task<Dictionary<string, ProductItem>> GetAllProducts()
    {
        if(_products.Count == 0) await LoadProducts();
        return _products;
    }

    private async Task LoadProducts()
    {
        List<Page> pages = await GetAllSitemapPages();

        
        const int batchSize = 50;
        for (int i = 0; i < pages.Count; i += batchSize)
        {
            var batch = pages.Skip(i).Take(batchSize);
            var productTasks = batch.Select(async page =>
            {
                List<ProductItem> products = await page.GetProducts();

                lock (_products)
                {
                    foreach (ProductItem product in products)
                    {
                        _products.TryAdd(product.Id, product);
                    }
                }
            });

            await Task.WhenAll(productTasks);
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }


    
}