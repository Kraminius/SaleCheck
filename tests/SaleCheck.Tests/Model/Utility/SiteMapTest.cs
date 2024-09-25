using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;

namespace SaleCheck.Tests.Model.Utility;

[TestSubject(typeof(SiteMap))]
public class SiteMapTest
{

    [Fact]
    public async Task CanFindPagesOnSiteMap()
    {
        string website = SampleSites.Sitemap;
        SiteMap siteMap = new SiteMap(website);
        await siteMap.LoadSiteMapsAsync();
        
        List<Page> pages = siteMap.GetPages();
        Assert.NotEmpty(pages);
        
    }
}