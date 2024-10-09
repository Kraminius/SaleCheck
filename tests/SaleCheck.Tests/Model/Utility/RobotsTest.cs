
using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;
using Xunit.Abstractions;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility
{
    [TestSubject(typeof(Robots))]
    public class RobotsTest
    {
        private readonly ITestOutputHelper _output;

        public RobotsTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task RobotsHasSiteMaps()
        {
            string title =  SampleSites.VinduesGrossisten.Title;
            string website = SampleSites.VinduesGrossisten.Link;
            Robots robots = new Robots(title, website);
            List<SiteMap> sitemaps = await robots.GetSiteMaps();
            Assert.NotEmpty(sitemaps);
        }

        
        [Fact]
        public async Task LoadAllProductsFromSiteMapPages()
        {
            string title =  SampleSites.VinduesGrossisten.Title;
            string website = SampleSites.VinduesGrossisten.Link;
            Robots robots = new Robots(title, website);
            _output.WriteLine("Created Robots: " + website);
            _output.WriteLine("Loading all pages... This takes about a minute.");
            List<Page> pages = await robots.GetAllSitemapPages();
            _output.WriteLine("Loaded All Pages: " + pages.Count);
            Assert.NotEmpty(pages);
            Dictionary<string, Product> products = new Dictionary<string, Product>();
            int index = 0;
            foreach (Page page in pages)
            {
                List<Product> pageProducts = await page.GetProducts();
                
                
                
                foreach (Product product in pageProducts)
                {
                    products.TryAdd(product.Id, product);
                }
                
                if(index%10 == 0) _output.WriteLine("From " + index + " pages: " + products.Count + " total products.");
                index++;
            }
            _output.WriteLine("######## DONE #########" );
            _output.WriteLine("Total Products: " + products.Count);

            int discounted = 0;
            foreach (var key in products.Keys)
            {
                if(products[key].OtherPrice != null) discounted++;
            }
            _output.WriteLine("Total Discounted: " + discounted + "/" + products.Count);
        }

        [Fact]
        public async Task LoadAllProductsForSpecificWebsite()
        {
            string title =  SampleSites.VinduesGrossisten.Title;
            string website = SampleSites.VinduesGrossisten.Link;
            Robots robots = new Robots(title, website);
            Dictionary<string, Product> products = await robots.GetAllProducts();
            Assert.NotEmpty(products);
        }
    }
    
}
