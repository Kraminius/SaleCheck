namespace SaleCheck.Model.Utility;

public interface IProductAnalyser
{
    public Task<List<ProductItem>> Analyze(Page page);
    public Task<List<Page>> GetRobotsSitemapLink(Page page);
    public Task<List<Page>> GetSitemapLinks(Page page);
}