using System.Text.RegularExpressions;

namespace SaleCheck.Model.Utility;

public class Robots(string title, string url)
{
    private Page? _page;
    private List<SiteMap> _siteMaps = new List<SiteMap>();
    private Dictionary<string, Product> _products = new Dictionary<string, Product>();
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
        string? html = await _page.GetHtmlContent();
        if (html == null) return; //returns if none is found
        IProductAnalyser analyser = ProductAnalyser.GetAnalyser(title);
        if (analyser == null) return;
        string[] sitemapLinks = await analyser.RegexMatchLinksFromRobots(_page);
        _siteMaps = new List<SiteMap>();
        foreach (string sitemapLink in sitemapLinks)
        {
            _siteMaps.Add(new SiteMap(title, sitemapLink));
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

    public async Task<Dictionary<string, Product>> GetAllProducts()
    {
        if(_products.Count == 0) await LoadProducts();
        return _products;
    }

    private async Task LoadProducts()
    {
        List<Page> pages = await GetAllSitemapPages();
        foreach (Page page in pages)
        {
            List<Product> products = await page.GetProducts();
            foreach (Product product in products)
            {
                _products.TryAdd(product.Id, product);
            }
        }
    }
    public async Task<List<Page>> GetAllOriginalPages()
    {
        List<string> allPageURls = new List<string>();
        List<SiteMap> sitemaps = await GetSiteMaps();
        foreach (SiteMap siteMap in sitemaps)              
        {
            await siteMap.LoadSiteMapsAsync();
            List<Page> siteMapPages = siteMap.GetPages();
            foreach (Page page in siteMapPages)
            {
                if(!allPageURls.Contains(page.GetUrl())) 
                    allPageURls.Add(page.GetUrl());
                string htmlContent = await page.GetHtmlContent();
                if (htmlContent == null) continue;
                List<Page> hrefs = await page.GetHrefs();
                // TODO scrape the html for useful products and save them to a database.
                foreach (Page href in hrefs)
                {
                    if(!allPageURls.Contains(href.GetUrl())) 
                        allPageURls.Add(href.GetUrl());
                }
            }
        }
        List<Page> allPages = new List<Page>();
        foreach (string pageUrl in allPageURls)
        {
            allPages.Add(new Page(title, pageUrl));
        }
        return allPages;
    }
}