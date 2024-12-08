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
    
    [Fact]
    public async Task CanFetchAllProducts()
    {
        string title = SampleSites.SportyFit.Title;
        List<string> categoryUrls = new List<string>
{
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/shop-by-activity/styrketraening/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/vaegtskiver/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/vaegtstang/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/vaegtstangssaet/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/kettlebell/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/racks-og-stativer/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/traeningsbaenk/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/racks-og-stativer/opbevaringsstativer/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/haandtag-og-greb/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/shop-by-activity/funktionel-traening/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/slyngetraener/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/foamroller/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/medicinbold/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/slamball/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/agility-stige/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/balancetraening/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/sjippetorv/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/mavehjul/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/gymnastikringe/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/jumpbox-plyo-box/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/prowler/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/battlerope/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/power-bags/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/traeningselastik/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/traeningsmaatte/yoga-maatte/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/traeningsmaatte/",
    "https://sportyfit.dk/vare-kategori/vandsports-udstyr/",
    "https://sportyfit.dk/vare-kategori/vandsports-udstyr/bananbaad/",
    "https://sportyfit.dk/vare-kategori/vandsports-udstyr/sups-paddleboards/",
    "https://sportyfit.dk/vare-kategori/vandsports-udstyr/kajak/",
    "https://sportyfit.dk/vare-kategori/vandsports-udstyr/svoemmeudstyr/badedyr/",
    "https://sportyfit.dk/vare-kategori/gode-tilbud/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/traeningsbaelte/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/fjederlaas/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/vaegtskiver/bumper-plates/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/boksepude-bokseudstyr/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/boksehandsker/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/wallball/",
    "https://sportyfit.dk/vare-kategori/cardio-maskiner/",
    "https://sportyfit.dk/vare-kategori/cardio-maskiner/loebebaand/",
    "https://sportyfit.dk/vare-kategori/cardio-maskiner/motionscykel/",
    "https://sportyfit.dk/vare-kategori/cardio-maskiner/romaskiner/",
    "https://sportyfit.dk/vare-kategori/cardio-maskiner/crosstrainer/",
    "https://sportyfit.dk/vare-kategori/cardio-maskiner/spinningcykler/",
    "https://sportyfit.dk/vare-kategori/sport-og-fritid/legetoej/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/gymnastikbold/traeningsbold/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/gulv-og-indretning/rfid-kort/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/haandvaegte/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/yoga-udstyr/",
    "https://sportyfit.dk/vare-kategori/traeningsudstyr/stepbaenk/"
};

        
        var allProducts = new List<ProductItem>();
        foreach (string url in categoryUrls)
        {
            Page page = new Page(title, url);

            _output.WriteLine($"Processing URL: {url}");

            try
            {
                List<ProductItem> products = await page.GetProducts();

                if (products.Count == 0)
                {
                    _output.WriteLine($"No products found for URL: {url}");
                }
                else
                {
                    _output.WriteLine($"Found {products.Count} products for URL: {url}");
                    foreach (ProductItem product in products)
                    {
                        _output.WriteLine(product.ToString());
                    }

                    allProducts.AddRange(products);
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error processing URL {url}: {ex.Message}");
            }
        }

        _output.WriteLine($"Total products fetched: {allProducts.Count}");

        // Assert that at least one product was found across all categories
        Assert.NotEmpty(allProducts);
    }

    
    [Fact]
    public async Task SaveAllProducts()
    {
        string title = SampleSites.SportyFit.Title;
        string url = SampleSites.SportyFit.Link;
        Robots robots = new Robots(title, url);
        Dictionary<string, ProductItem> products = await robots.GetAllProducts();
        string dateString = DateTime.Now.ToString("dd-MM-yyyy");
        string fileName = $"{dateString}-{title}.csv";

        string filePath = Path.Combine("C:\\Users\\boots\\RiderProjects\\SaleCheck", fileName); // Update the path as needed
        
        Assert.True(CSVExporter.SaveProductsToCsv(products, filePath));

    }
}