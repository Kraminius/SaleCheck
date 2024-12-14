using SaleCheck.Repositories.Interfaces;

namespace SaleCheck.Utility
{
    public class PriceCleanupService
    {
        private readonly IWebsiteRepository _websiteRepository;
        private readonly ILogger<PriceCleanupService> _logger;

        public PriceCleanupService(IWebsiteRepository websiteRepository, ILogger<PriceCleanupService> logger)
        {
            _websiteRepository = websiteRepository;
            _logger = logger;
        }

        public async Task CleanUpPricesAsync()
        {
            try
            {
                _logger.LogInformation("Starting price cleanup...");

                var websites = await _websiteRepository.GetAllWebsitesAsync();

                foreach (var website in websites)
                {
                    foreach (var product in website.Products)
                    {
                        bool isUpdated = false;

                        foreach (var priceEntry in product.Price)
                        {
                            if (priceEntry.DiscountPrice.HasValue && priceEntry.DiscountPrice > priceEntry.NormalPrice)
                            {
                                // Swap the prices
                                _logger.LogWarning(
                                    $"Price issue found in Product ID: {product.ProductId}, Website ID: {website.WebsiteId}. Fixing...");
                                (priceEntry.NormalPrice, priceEntry.DiscountPrice) = (priceEntry.DiscountPrice.Value, priceEntry.NormalPrice);
                                isUpdated = true;
                            }

                        }

                        if (isUpdated)
                        {
                            await _websiteRepository.UpdateWebsiteAsync(website.WebsiteId, website);
                            _logger.LogInformation(
                                $"Prices corrected for Product ID: {product.ProductId}, Website ID: {website.WebsiteId}");
                        }
                    }
                }

                _logger.LogInformation("Price cleanup completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during price cleanup.");
                throw;
            }
        }
    }
}