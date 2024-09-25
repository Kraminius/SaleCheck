using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;
using Xunit.Abstractions;

namespace SaleCheck.Tests.Model.Utility
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
            List<SiteMap> sitemaps = await robots.GetSiteMaps();
            int totalPages = 0;
            foreach (SiteMap siteMap in sitemaps)              
            {
                _output.WriteLine("----------------------------");
                _output.WriteLine("Loading Sitemap from Link: " + siteMap.GetLink());
                _output.WriteLine("----------------------------");
                await siteMap.LoadSiteMapsAsync();
                List<Page> siteMapPages = siteMap.GetPages();
                Assert.NotEmpty(siteMapPages);
                totalPages += siteMapPages.Count;
                _output.WriteLine("Pages in Sitemap: " + siteMapPages.Count.ToString());
                foreach (Page page in siteMapPages)
                {
                    _output.WriteLine("Loading Page: " + page.GetUrl());
                    string htmlContent = await page.GetHtmlContent();
                    if (htmlContent == null) continue;
                    // TODO scrape the html for useful products and save them to a database.
                    
                }
            }
            _output.WriteLine("Total Pages: " + totalPages);
        }
    }
}
