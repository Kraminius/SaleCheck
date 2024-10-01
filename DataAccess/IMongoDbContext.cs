using MongoDB.Driver;
using SaleCheck.Model.DataClasses;

namespace SaleCheck.DataAccess.Interfaces
{
    public class IMongoDbContext
    {
        public IMongoCollection<Website> Websites { get; }
    }
}
