namespace SaleCheck.Model.DataClasses
{
    public class Website
    {
        public required string WebsiteId { get; set; }
        public required string WebsiteName { get; set; }
        public required string WebsiteUrl { get; set; }

        // Dictionary to store sitemap URL and its hashed value (to see if it changed since last visit)
        public required Dictionary<string, string> Sitemap { get; set; } = new Dictionary<string, string>();

        public string? WebsiteType { get; set; }
        public List<Product>? Products { get; set; }
    }
}
