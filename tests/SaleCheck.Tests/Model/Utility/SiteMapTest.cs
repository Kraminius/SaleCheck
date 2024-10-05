using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;

[TestSubject(typeof(SiteMap))]
public class SiteMapTest
{

    [Fact]
    public async Task CanFindPagesOnSiteMap()
    {
        string title =  SampleSites.VinduesGrossisten.Title;
        string website = SampleSites.VinduesGrossisten.Link;
        Robots robots = new Robots(title, website);
        List<SiteMap> siteMap = await robots.GetSiteMaps();
        await siteMap[0].LoadSiteMapsAsync();
        List<Page> pages = siteMap[0].GetPages();
        Assert.NotEmpty(pages);
        
    }
}