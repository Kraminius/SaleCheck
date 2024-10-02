using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
            string website = SampleSites.Links[0];
            Robots robots = new Robots(website);
            List<SiteMap> sitemaps = await robots.GetSiteMaps();
            Assert.NotEmpty(sitemaps);
        }

        [Fact]
        public async Task LoadAllPagesFromRobots()
        {
            string website = SampleSites.Links[0];
            _output.WriteLine("############################");
            _output.WriteLine("Loading pages from Link: " + website);
            _output.WriteLine("############################");
            Robots robots = new Robots(website);
            List<string> allPages = new List<string>();
            List<SiteMap> sitemaps = await robots.GetSiteMaps();
            int totalPages = 0;
            int hrefPages = 0;
            foreach (SiteMap siteMap in sitemaps)              
            {
                _output.WriteLine("----------------------------");
                _output.WriteLine("Loading Sitemap from Link: " + siteMap.GetLink());
                _output.WriteLine("----------------------------");
                await siteMap.LoadSiteMapsAsync();
                List<Page> siteMapPages = siteMap.GetPages();
                Assert.NotEmpty(siteMapPages);
                totalPages += siteMapPages.Count;
                foreach (Page page in siteMapPages)
                {
                    if(!allPages.Contains(page.GetUrl())) 
                        allPages.Add(page.GetUrl());
                    string htmlContent = await page.GetHtmlContent();
                    if (htmlContent == null) continue;
                    List<Page> hrefs = await page.GetHrefs();
                    hrefPages += hrefs.Count;
                    // TODO scrape the html for useful products and save them to a database.
                    foreach (Page href in hrefs)
                    {
                        if(!allPages.Contains(href.GetUrl())) 
                            allPages.Add(href.GetUrl());
                    }
                }
            }
            _output.WriteLine("Total Pages: " + totalPages);
            _output.WriteLine("Total Href Pages: " + hrefPages);
            _output.WriteLine("Total Non-Duplicate Pages: " + allPages.Count.ToString());
        }
        
        [Fact]
        public async Task LoadAllProductsFromRobots()
        {
            string website = SampleSites.Links[0];
            Robots robots = new Robots(website);
            _output.WriteLine("Created Robots: " + website);
            _output.WriteLine("Loading all pages... This takes about a minute.");
            List<Page> pages = await robots.GetAllOriginalPages();
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
                
                if(index%100 == 0) _output.WriteLine("From " + index + " pages: " + products.Count + " total products.");
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
    }
}
