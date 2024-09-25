using System.Threading.Tasks;
using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;
using Xunit.Abstractions;

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;

[TestSubject(typeof(Page))]
public class PageTest
{

    private readonly ITestOutputHelper _output;

    
    public PageTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task CanLoadWebsites()
    {
        string website = SampleSites.Links[0];        
        Page page = new Page(website);
        string content = await page.GetHtmlContent();
        
        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }
    
    [Fact]
    public async Task CanFindHrefs()
    {
        string website = SampleSites.ProductSite;        
        Page page = new Page(website);
        string? content = await page.GetHtmlContent();
        if(content == null) _output.WriteLine("No content found in page");
        else _output.WriteLine("Content found in page");
        List<Page> pages = await page.GetHrefs();
        _output.WriteLine("Generated Pages: " + pages.Count.ToString());
        Assert.NotNull(pages);
    }
}