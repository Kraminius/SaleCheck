

namespace SaleCheck.Tests.SaleCheck.Tests.Model.Utility;

public interface IWebsiteTest
{
    public Task CanLoadPage();
    public Task CanFindRobots();
    public Task CanFindSitemap();
    public Task CanFindSitemapPages();
    public Task CanFindProducts();
    public Task CanFindAllProducts();
}