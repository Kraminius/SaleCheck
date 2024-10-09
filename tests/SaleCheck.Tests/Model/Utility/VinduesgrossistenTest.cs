using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;
using Xunit.Abstractions;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;
[TestSubject(typeof(Robots))]
public class VinduesgrossistenTest : IWebsiteTest
{
    private readonly ITestOutputHelper _output;

    
    public VinduesgrossistenTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task CanLoadPage()
    {
        string title = SampleSites.VinduesGrossisten.Title;
        string url = SampleSites.VinduesGrossisten.Link;
        Page page = new Page(title, url);
        string content = await page.GetHtmlContent();
        Assert.NotNull(content);
    }
    [Fact]
    public async Task CanFindRobots()
    {
        string title = SampleSites.VinduesGrossisten.Title;
        string url = SampleSites.VinduesGrossisten.Link;
        Robots robots = new Robots(title, url);
        Assert.True(await robots.LoadRobots());
    }
    [Fact]
    public async Task CanFindSitemap()
    {
        string title = SampleSites.VinduesGrossisten.Title;
        string url = SampleSites.VinduesGrossisten.Link;
        Robots robots = new Robots(title, url);
        List<SiteMap> sitemaps = await robots.GetSiteMaps();
        Assert.NotNull(sitemaps);
        Assert.NotEmpty(sitemaps);
    }
    [Fact]
    public async Task CanFindSitemapPages()
    {
        string title = SampleSites.VinduesGrossisten.Title;
        string url = SampleSites.VinduesGrossisten.Link;
        Robots robots = new Robots(title, url);
        List<Page> pages = await robots.GetAllSitemapPages();
        Assert.NotNull(pages);
        Assert.NotEmpty(pages);
        _output.WriteLine("Pages found: " + pages.Count);
    }
    [Fact]
    public async Task CanFindProducts()
    {
        string title = SampleSites.VinduesGrossisten.Title;
        string url = SampleSites.VinduesGrossisten.CateogrySite;
        Page page = new Page(title, url);
        List<ProductItem> products = await page.GetProducts();
        foreach (ProductItem product in products)
        {
            _output.WriteLine(product.ToString());
        }
        Assert.NotEmpty(products);
    }
    [Fact]
    public async Task CanFindAllProducts()
    {
        string title = SampleSites.VinduesGrossisten.Title;
        string url = SampleSites.VinduesGrossisten.Link;
        Robots robots = new Robots(title, url);
        Dictionary<string, ProductItem> products = await robots.GetAllProducts();
        Assert.NotEmpty(products);
        _output.WriteLine("products: " + products.Count);
    }
    [Fact]
    public async Task SaveAllProducts()
    {
        string title = SampleSites.VinduesGrossisten.Title;
        string url = SampleSites.VinduesGrossisten.Link;
        Robots robots = new Robots(title, url);
        Dictionary<string, ProductItem> products = await robots.GetAllProducts();
        string dateString = DateTime.Now.ToString("dd-MM-yyyy");
        string fileName = $"{dateString}-{title}.csv";

        string filePath = Path.Combine("C:\\Users\\tobia\\OneDrive\\Skrivebord\\TestData", fileName); // Update the path as needed
        
        Assert.True(CSVExporter.SaveProductsToCsv(products, filePath));

    }


}