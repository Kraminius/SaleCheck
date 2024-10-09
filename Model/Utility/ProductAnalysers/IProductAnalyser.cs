namespace SaleCheck.Model.Utility;

public interface IProductAnalyser
{
    public Task<List<Product>> Analyze(Page page);
    public Task<List<Page>> GetRobotsSitemapLink(Page page);
    public Task<List<Page>> GetSitemapLinks(Page page);
}