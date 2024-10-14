using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using SaleCheck.DataAccess.Interfaces;
using SaleCheck.Model.DataClasses;
using SaleCheck.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SaleCheck.Repositories
{
    public class WebsiteRepository : IWebsiteRepository
    {
        private readonly IMongoCollection<Website> _websites;
        private readonly ILogger<WebsiteRepository> _logger;


        public WebsiteRepository(IMongoDbContext context, ILogger<WebsiteRepository> logger)
        {
            _websites = context.Websites;
            _logger = logger;
        }

        #region Website Operations
        public async Task<IEnumerable<Website>> GetAllWebsitesAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation($"Getting all websites, page number: {pageNumber}, page size: {pageSize}");
                return await _websites.Find(website => true)
                    .Skip((pageNumber - 1) * pageSize)
                    .Limit(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting all websites");
                throw;
            }
        }

        public async Task<Website> GetWebsiteByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Getting website by id: {id}");
                return await _websites.Find(website => website.WebsiteId.Equals(id))
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting website by id");
                throw;
            }
        }
        
        public async Task CreateWebsiteAsync(Website website)
        {
            try
            {
                Console.WriteLine("Creating website");
                _logger.LogInformation($"Creating website.");
                await _websites.InsertOneAsync(website);
                Console.WriteLine("Website created successfully");
                _logger.LogInformation("Website created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating website");
                Console.WriteLine("Error occured while creating website: " + ex.Message);
                throw;
            }
        }
        
        public async Task UpdateWebsiteAsync(string id, Website website)
        {
            try
            {
                _logger.LogInformation($"Updating website with id: {id}");
                var result = await _websites.ReplaceOneAsync(w => w.WebsiteId.Equals(id), website);
                if (result.MatchedCount == 0)
                {
                    _logger.LogWarning($"Website with id: {id} not found");
                    throw new KeyNotFoundException($"Website with id: {id} not found"); 
                }
                _logger.LogInformation("Website updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating website");
                throw;
            }
        }
        
        public async Task DeleteWebsiteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting website with ID: {id}");
                var result = await _websites.DeleteOneAsync(w => w.WebsiteId.Equals(id));
                if (result.DeletedCount == 0)
                {
                    _logger.LogWarning($"Website with ID: {id} not found for deletion.");
                    throw new KeyNotFoundException($"Website with ID: {id} not found.");
                }
                _logger.LogInformation("Website deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting website with ID: {id}");
                throw;
            }
        }

        #endregion Website Operations
        
        #region Product Operations
        
        public async Task<IEnumerable<Product>> GetProductsByWebsiteIdAsync(string websiteId)
        {
            try
            {
                _logger.LogInformation($"Getting products for website with ID: {websiteId}");
                var website = await _websites.Find(w => w.WebsiteId.Equals(websiteId))
                    .FirstOrDefaultAsync();
                return website.Products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting products for website with ID: {websiteId}");
                throw;
            }
        }
        
        public async Task<Product> GetProductByIdAsync(string websiteId, string productId)
        {
            try
            {
                _logger.LogInformation($"Getting product with ID: {productId} for website with ID: {websiteId}");
                var website = await _websites.Find(w => w.WebsiteId.Equals(websiteId)).FirstOrDefaultAsync();

                if (website == null)
                {
                    _logger.LogWarning($"Website with ID: {websiteId} not found");
                    throw new KeyNotFoundException($"Website with ID: {websiteId} not found");
                }
                
                var product = website.Products.FirstOrDefault(p => p.ProductId.Equals(productId));

                if (product == null)
                {
                    _logger.LogWarning($"Product with ID: {productId} not found");
                    throw new KeyNotFoundException($"Product with ID: {productId} not found");
                }

                return product;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while getting product with ID: {productId} for website with ID: {websiteId}");
                throw;
            }
        }
        
        public async Task CreateProductAsync(string websiteId, Product product)
        {
            try
            {
                _logger.LogInformation($"Creating a new product for website ID: {websiteId}");
                var update = Builders<Website>.Update.Push(w => w.Products, product);
                var result = await _websites.UpdateOneAsync(w => w.WebsiteId.Equals(websiteId), update);
                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning($"Website with ID: {websiteId} not found for adding product.");
                    throw new KeyNotFoundException($"Website with ID: {websiteId} not found.");
                }
                _logger.LogInformation("Product created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating product for website ID: {websiteId}");
                throw;
            }
        }
        
        public async Task UpdateProductAsync(string websiteId, string productId, Product product)
        {
            try
            {
                _logger.LogInformation($"Updating product ID: {productId} for website ID: {websiteId}");
                var filter = Builders<Website>.Filter.Eq(w => w.WebsiteId, websiteId) &
                             Builders<Website>.Filter.ElemMatch(w => w.Products, p => p.ProductId == productId);
                var update = Builders<Website>.Update.Set(w => w.Products[-1], product);
                var result = await _websites.UpdateOneAsync(filter, update);
                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning($"Product ID: {productId} not found in website ID: {websiteId}.");
                    throw new KeyNotFoundException($"Product ID: {productId} not found in website ID: {websiteId}.");
                }
                _logger.LogInformation("Product updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating product ID: {productId} for website ID: {websiteId}");
                throw;
            }
        }

        public async Task DeleteProductAsync(string websiteId, string productId)
        {
            try
            {
                _logger.LogInformation($"Deleting product ID: {productId} from website ID: {websiteId}");
                var update = Builders<Website>.Update.PullFilter(w => w.Products, p => p.ProductId.Equals(productId));
                var result = await _websites.UpdateOneAsync(w => w.WebsiteId.Equals(websiteId), update);
                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning($"Product ID: {productId} not found in website ID: {websiteId}.");
                    throw new KeyNotFoundException($"Product ID: {productId} not found in website ID: {websiteId}.");
                }
                _logger.LogInformation("Product deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting product ID: {productId} from website ID: {websiteId}");
                throw;
            }
        }
        
        #endregion Product Operations
        
        #region Subsite Operations
        
        public async Task<IEnumerable<Subsite>> GetSubsitesByWebsiteIdAsync(string websiteId)
        {
            try
            {
                _logger.LogInformation($"Fetching subsites for website ID: {websiteId}");
                var website = await _websites.Find(w => w.WebsiteId.Equals(websiteId)).FirstOrDefaultAsync();
                return website?.Subsites ?? new List<Subsite>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching subsites for website ID: {websiteId}");
                throw;
            }
        }
        
        public async Task<Subsite> GetSubsiteByUrlAsync(string websiteId, string url)
        {
            try
            {
                _logger.LogInformation($"Fetching subsite URL: {url} for website ID: {websiteId}");
                var website = await _websites.Find(w => w.WebsiteId.Equals(websiteId)).FirstOrDefaultAsync();
                return website?.Subsites.FirstOrDefault(s => s.Url.Equals(url));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching subsite URL: {url} for website ID: {websiteId}");
                throw;
            }
        }
        
         public async Task CreateSubsiteAsync(string websiteId, Subsite subsite)
        {
            try
            {
                _logger.LogInformation($"Creating a new subsite for website ID: {websiteId}");
                var update = Builders<Website>.Update.Push(w => w.Subsites, subsite);
                var result = await _websites.UpdateOneAsync(w => w.WebsiteId.Equals(websiteId), update);
                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning($"Website with ID: {websiteId} not found for adding subsite.");
                    throw new KeyNotFoundException($"Website with ID: {websiteId} not found.");
                }
                _logger.LogInformation("Subsite created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while creating subsite for website ID: {websiteId}");
                throw;
            }
        }

        public async Task UpdateSubsiteAsync(string websiteId, string url, Subsite subsite)
        {
            try
            {
                _logger.LogInformation($"Updating subsite URL: {url} for website ID: {websiteId}");
                var filter = Builders<Website>.Filter.Eq(w => w.WebsiteId, websiteId) &
                             Builders<Website>.Filter.ElemMatch(w => w.Subsites, s => s.Url == url);
                var update = Builders<Website>.Update.Set(w => w.Subsites[-1], subsite);
                var result = await _websites.UpdateOneAsync(filter, update);
                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning($"Subsite URL: {url} not found in website ID: {websiteId}.");
                    throw new KeyNotFoundException($"Subsite URL: {url} not found in website ID: {websiteId}.");
                }
                _logger.LogInformation("Subsite updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating subsite URL: {url} for website ID: {websiteId}");
                throw;
            }
        }

        public async Task DeleteSubsiteAsync(string websiteId, string url)
        {
            try
            {
                _logger.LogInformation($"Deleting subsite URL: {url} from website ID: {websiteId}");
                var update = Builders<Website>.Update.PullFilter(w => w.Subsites, s => s.Url.Equals(url));
                var result = await _websites.UpdateOneAsync(w => w.WebsiteId.Equals(websiteId), update);
                if (result.ModifiedCount == 0)
                {
                    _logger.LogWarning($"Subsite URL: {url} not found in website ID: {websiteId}.");
                    throw new KeyNotFoundException($"Subsite URL: {url} not found in website ID: {websiteId}.");
                }
                _logger.LogInformation("Subsite deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting subsite URL: {url} from website ID: {websiteId}");
                throw;
            }
        }
        
        #endregion Subsite Operations


    }
}
