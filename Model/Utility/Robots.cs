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
        if (_page == null) if(!await LoadRobots()) return; 
        string? html = await _page.GetHtmlContent() ?? null;
        if (html == null) return; //returns if none is found
        IProductAnalyser? analyser = ProductAnalyser.GetAnalyser(title) ?? null;
        if (analyser == null) return;
        List<Page> sitemapLinks = await analyser.GetRobotsSitemapLink(_page);
        _siteMaps = new List<SiteMap>();
        foreach (Page sitemapLink in sitemapLinks)
        {
            _siteMaps.Add(new SiteMap(title, sitemapLink.GetUrl()));
        }
    }
    
    

    public async Task<List<Page>> GetAllSitemapPages()
    {
        List<Page> allPages = new List<Page>();
        List<SiteMap> sitemaps = await GetSiteMaps();
        foreach (SiteMap siteMap in sitemaps)              
        {
            await siteMap.LoadSiteMapsAsync();
            List<Page> siteMapPages = siteMap.GetPages();
            allPages.AddRange(siteMapPages);
        }
        return allPages;
    }

    public async Task<Dictionary<string, ProductItem>> GetAllProducts()
    {
        if(_products.Count == 0) await LoadProducts();
        return _products;
    }

    private async Task LoadProducts()
    {
        List<Page> pages = await GetAllSitemapPages();
        foreach (Page page in pages)
        {
            List<ProductItem> products = await page.GetProducts();
            foreach (ProductItem product in products)
            {
                _products.TryAdd(product.Id, product);
            }
        }
    }
    
}