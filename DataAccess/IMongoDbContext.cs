using MongoDB.Driver;
using SaleCheck.Model.DataClasses;

namespace SaleCheck.DataAccess.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<Website> Websites { get; }
    }
}
