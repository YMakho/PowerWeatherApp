using MongoDB.Driver;
using PowerWeatherApp.Infrastructure.Persistence.MongoEntities;

namespace PowerWeatherApp.Infrastructure.Persistence.Interface
{
    public interface IMongoDbContext
    {
        IMongoDatabase Database { get; }
        IMongoCollection<CachedForecast> GetWeatherCollection();   
    }
}
