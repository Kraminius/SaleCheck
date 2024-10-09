namespace SaleCheck.Model.DataClasses
{
    public class Product
    {
        public required string ProductId { get; set; }
        public string? ProductName { get; set; }
        public required string OldPrice { get; set; }
        public required string NewPrice { get; set; }
    }
}
