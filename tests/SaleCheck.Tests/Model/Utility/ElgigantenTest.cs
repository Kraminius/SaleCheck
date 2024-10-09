using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;
using Xunit.Abstractions;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;
[TestSubject(typeof(Robots))]
public class ElgigantenTest : IWebsiteTest
{
    private readonly ITestOutputHelper _output;

    
    public ElgigantenTest(ITestOutputHelper output)
    {
        _output = output;
    }
    
    
    [Fact]
    public async Task CanLoadPage()
    {
        string title = SampleSites.Elgiganten.Title;
        string url = SampleSites.Elgiganten.Link;
        Page page = new Page(title, url);
        string content = await page.GetHtmlContent();
        Assert.NotNull(content);
    }
    [Fact]
    public async Task CanFindRobots()
    {
        string title = SampleSites.Elgiganten.Title;
        string url = SampleSites.Elgiganten.Link;
        Robots robots = new Robots(title, url);
        Assert.True(await robots.LoadRobots());
    }
    [Fact]
    public async Task CanFindSitemap()
    {
        string title = SampleSites.Elgiganten.Title;
        string url = SampleSites.Elgiganten.Link;
        Robots robots = new Robots(title, url);
        List<SiteMap> sitemaps = await robots.GetSiteMaps();
        Assert.NotNull(sitemaps);
        Assert.NotEmpty(sitemaps);
    }
    [Fact]
    public async Task CanFindSitemapPages()
    {
        string title = SampleSites.Elgiganten.Title;
        string url = SampleSites.Elgiganten.Link;
        Robots robots = new Robots(title, url);
        List<Page> pages = await robots.GetAllSitemapPages();
        Assert.NotNull(pages);
        Assert.NotEmpty(pages);
        _output.WriteLine("Pages found: " + pages.Count);
        foreach (Page page in pages)
        {
            _output.WriteLine(page.GetUrl());
        }
    }
    [Fact]
    public async Task CanFindProducts()
    {
        string title = SampleSites.Elgiganten.Title;
        string url = SampleSites.Elgiganten.CateogrySite;
        Page page = new Page(title, url);
        List<Product> products = await page.GetProducts();
        foreach (Product product in products)
        {
            _output.WriteLine(product.ToString());
        }
        Assert.NotEmpty(products);
    }

    [Fact]
    public async Task CanFindAllProducts()
    {
        string title = SampleSites.Elgiganten.Title;
        string url = SampleSites.Elgiganten.Link;
        Robots robots = new Robots(title, url);
        Dictionary<string, Product> products = await robots.GetAllProducts();
        Assert.NotEmpty(products);
        _output.WriteLine("products: " + products.Count);
    }
    
}