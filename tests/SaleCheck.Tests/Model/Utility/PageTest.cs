using System.Threading.Tasks;
using JetBrains.Annotations;
using SaleCheck.Model.Utility;
using Xunit;

namespace SaleCheck.Tests.Model.Utility;

[TestSubject(typeof(Page))]
public class PageTest
{

    [Fact]
    public async Task CanLoadWebsites()
    {
        string website = SampleSites.Links[0];        
        Page page = new Page(website);
        string content = await page.GetHtmlContent();
        
        Assert.NotNull(content);
        Assert.NotEmpty(content);
    }
}