
namespace SaleCheck.HtmlLib;
public class Rules
{
    //Website info
    public required string WebsiteId { get; set; }
    public required string WebsiteUrl { get; set; }
    public required string ExampleHtml { get; set; }
    //Commands:
    public required string ParentTagRule { get; set; }
    public required string NameRule { get; set; }
    public required string IdRule { get; set; }
    public required string NormalPriceRule { get; set; }
    public required string DiscountPriceRule { get; set; }
    public required string NoDiscountPriceRule { get; set; }
    public required string LinkRule { get; set; }
}