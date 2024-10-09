using SaleCheck.Model.DataClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaleCheck.Repositories.Interfaces
{
    public interface IWebsiteRepository
    {
        // Website Operations
        Task<IEnumerable<Website>> GetAllWebsitesAsync(int pageNumber = 1, int pageSize = 10);
        Task<Website> GetWebsiteByIdAsync(int id);
        Task CreateWebsiteAsync(Website website);
        Task UpdateWebsiteAsync(int id, Website website);
        Task DeleteWebsiteAsync(int id);

        // Product Operations
        Task<IEnumerable<Product>> GetProductsByWebsiteIdAsync(int websiteId);
        Task<Product> GetProductByIdAsync(int websiteId, int productId);
        Task CreateProductAsync(int websiteId, Product product);
        Task UpdateProductAsync(int websiteId, int productId, Product product);
        Task DeleteProductAsync(int websiteId, int productId);

        // Subsite Operations
        Task<IEnumerable<Subsite>> GetSubsitesByWebsiteIdAsync(int websiteId);
        Task<Subsite> GetSubsiteByUrlAsync(int websiteId, string url);
        Task CreateSubsiteAsync(int websiteId, Subsite subsite);
        Task UpdateSubsiteAsync(int websiteId, string url, Subsite subsite);
        Task DeleteSubsiteAsync(int websiteId, string url);
    }
}
