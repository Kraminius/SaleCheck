namespace SaleCheck.Model
{
    public class Website
    {
        public required string WebsiteId { get; set; }
        public required string WebsiteName { get; set; }
        public required string WebsiteUrl { get; set; }
        public string? WebsiteType { get; set; }
        public List<Product>? Products { get; set; }
    }
}
