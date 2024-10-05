namespace SaleCheck.Model.Utility;

public interface IProductAnalyser
{
    public Task<List<Product>> Analyze(Page page);
    public Task<string[]> RegexMatchLinksFromRobots(Page page);
    public Task<string[]> RegexMatchLinksFromSitemap(Page page);
}