using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SaleCheck.Model.DataClasses;
using SaleCheck.DataAccess.Interfaces;

namespace SaleCheck.DataAccess
{
    public class MongoDbContext : IMongoDbContext
    {

        private readonly IMongoDatabase _database;
        public IMongoCollection<ScraperWebsiteRules> ScraperWebsiteRules => _database.GetCollection<ScraperWebsiteRules>("ScraperWebsiteRules");


        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Website> Websites => _database.GetCollection<Website>("Websites");

    }
}
