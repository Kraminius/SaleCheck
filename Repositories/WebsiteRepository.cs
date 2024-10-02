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
    public class WebsiteRepository
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

        public async Task<Website> GetWebsiteByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Getting website by id: {id}");
                return await _websites.Find(website => website.WebsiteId == id)
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
                _logger.LogInformation($"Creating website.");
                await _websites.InsertOneAsync(website);
                _logger.LogInformation("Website created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating website");
                throw;
            }
        }
        
        public async Task UpdateWebsiteAsync(int id, Website website)
        {
            try
            {
                _logger.LogInformation($"Updating website with id: {id}");
                var result = await _websites.ReplaceOneAsync(w => w.WebsiteId == id, website);
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
        
        public async Task DeleteWebsiteAsync(int id)
        {
            try
            {
                _logger.LogInformation($"Deleting website with ID: {id}");
                var result = await _websites.DeleteOneAsync(w => w.WebsiteId == id);
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
        
        //TODO: implement product operations
        
        #endregion Product Operations












    }
}
