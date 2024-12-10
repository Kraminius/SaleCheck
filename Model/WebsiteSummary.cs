namespace SaleCheck.Model.DataClasses;

public class WebsiteSummary
{
    public string WebsiteId { get; set; }
    public string WebsiteUrl { get; set; }
    public string WebsiteName { get; set; }
    public int ProductsCount { get; set; }
    public int ProductsOnSale { get; set; }
    public int ProductsViolation { get; set; }
    public int ProductsLongestCurrentStreak { get; set; }
}