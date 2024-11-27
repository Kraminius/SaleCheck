using MongoDB.Driver;
using SaleCheck.Model.DataClasses;
using SaleCheck.DataAccess.Interfaces;
using System.Threading.Tasks;

namespace SaleCheck.DataAccess
{
    public class ScraperWebsiteRulesRepository
    {
        private readonly IMongoDbContext _context;

        public ScraperWebsiteRulesRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(ScraperWebsiteRules rules)
        {
            await _context.ScraperWebsiteRules.InsertOneAsync(rules);
        }

        public async Task<List<ScraperWebsiteRules>> GetAllAsync()
        {
            return await _context.ScraperWebsiteRules.Find(_ => true).ToListAsync();
        }
    }
}