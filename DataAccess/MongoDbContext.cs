using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SaleCheck.Model.DataClasses;
using SaleCheck.DataAccess.Interfaces;

namespace SaleCheck.DataAccess
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            EnsureCollectionExists("Websites");
        }

        public IMongoCollection<Website> Websites => _database.GetCollection<Website>("Websites");

        private void EnsureCollectionExists(string collectionName)
        {
            var collectionNames = _database.ListCollectionNames().ToList();

            if (!collectionNames.Contains(collectionName))
            {
                _database.CreateCollection(collectionName);
                Console.WriteLine($"Collection '{collectionName}' has been created.");
            }
        }
    }
}