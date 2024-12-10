using JetBrains.Annotations;
using SaleCheck.HtmlLib;
using SaleCheck.Model.Utility;
using SaleCheck.Model.Utility.ProductAnalysers;
using Xunit;
using Xunit.Abstractions;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;
[TestSubject(typeof(Robots))]
public class SportyFit : IWebsiteTest
{
    private readonly ITestOutputHelper _output;

    
    public SportyFit(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task CanLoadPage()
    {
        string title = SampleSites.SportyFit.Title;
        string url = SampleSites.SportyFit.Link;
        Page page = new Page(title, url);
        string content = await page.GetHtmlContent();
        Assert.NotNull(content);
    }
    [Fact]
    public async Task CanFindRobots()
    {
        string title = SampleSites.SportyFit.Title;
        string url = SampleSites.SportyFit.Link;
        Robots robots = new Robots(title, url);

        Assert.True(await robots.LoadRobots());
        
        string? content = await robots.GetRobotsContent();
        if (content != null)
        {
            _output.WriteLine(content);
        }
        else
        {
            _output.WriteLine("Contents of robots not found.");
        }
        Assert.NotNull(content);
        
        
        
        
    }
    [Fact]
    public async Task CanFindSitemap()
    {
        string title = SampleSites.SportyFit.Title;
        string url = SampleSites.SportyFit.Link;
        Robots robots = new Robots(title, url);
        List<SiteMap> sitemaps = await robots.GetSiteMaps();
        Assert.NotNull(sitemaps);
        Assert.NotEmpty(sitemaps);
        foreach (SiteMap sitemap in sitemaps)
        {
            _output.WriteLine(sitemap.GetLink());
        }
    }
    [Fact]
    public async Task CanFindSitemapPages()
    {
        string title = SampleSites.SportyFit.Title;
        string url = SampleSites.SportyFit.Link;
        Robots robots = new Robots(title, url);
        List<Page> pages = await robots.GetAllSitemapPages();
        Assert.NotNull(pages);
        Assert.NotEmpty(pages);
        _output.WriteLine("Pages found: " + pages.Count);
    }
    
    [Fact]
    public async Task CanFindProducts()
    {
        string title = SampleSites.SportyFit.Title;
        string url = SampleSites.SportyFit.CategorySite;
        Page page = new Page(title, url);
        List<ProductItem> products = await page.GetProducts();
        foreach (ProductItem product in products)
        {
            _output.WriteLine(product.ToString());
        }
        string? content = await page.GetHtmlContent();
        if (content != null)
        {
            Html html = new Html(content);
            //_output.WriteLine("Trimmed Content:\n" + html.StringHtml);
            _output.WriteLine("Html toString:\n" + html.ToString());
            
        }
            
        
        Assert.NotEmpty(products);
    }
    
    [Fact]
    public async Task CanFindAllProducts()
    {
        string title = SampleSites.SportyFit.Title;
        string url = SampleSites.SportyFit.Link;
        Robots robots = new Robots(title, url);
        Dictionary<string, ProductItem> products = await robots.GetAllProducts();
        Assert.NotEmpty(products);
        _output.WriteLine("products: " + products.Count);
    }
    
}