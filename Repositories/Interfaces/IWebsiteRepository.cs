using SaleCheck.Model.DataClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaleCheck.Repositories.Interfaces
{
    public interface IWebsiteRepository
    {
        // Website Operations
        Task<IEnumerable<Website>> GetAllWebsitesAsync(int pageNumber = 1, int pageSize = 10);
        Task<Website> GetWebsiteByIdAsync(string id);
        Task CreateWebsiteAsync(Website website);
        Task UpdateWebsiteAsync(string id, Website website);
        Task DeleteWebsiteAsync(string id);

        // Product Operations
        Task<IEnumerable<Product>> GetProductsByWebsiteIdAsync(string websiteId);
        Task<Product> GetProductByIdAsync(string websiteId, string productId);
        Task CreateProductAsync(string websiteId, Product product);
        Task UpdateProductAsync(string websiteId, string productId, Product product);
        Task DeleteProductAsync(string websiteId, string productId);

        // Subsite Operations
        Task<IEnumerable<Subsite>> GetSubsitesByWebsiteIdAsync(string websiteId);
        Task<Subsite> GetSubsiteByUrlAsync(string websiteId, string url);
        Task CreateSubsiteAsync(string websiteId, Subsite subsite);
        Task UpdateSubsiteAsync(string websiteId, string url, Subsite subsite);
        Task DeleteSubsiteAsync(string websiteId, string url);
    }
}
