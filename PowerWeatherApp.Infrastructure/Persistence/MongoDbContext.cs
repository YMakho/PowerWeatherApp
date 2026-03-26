using MongoDB.Driver;
using PowerWeatherApp.Infrastructure.Persistence.Interface;
using PowerWeatherApp.Infrastructure.Persistence.MongoEntities;

namespace PowerWeatherApp.Application
{
    public sealed class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;
        public MongoDbContext(IMongoDatabase database, string collectionName)
        {
            _database = database;
            _collectionName = collectionName;
        }
        public IMongoDatabase Database { get { return _database; } }

        public IMongoCollection<CachedForecast> GetWeatherCollection()
        {
            return _database.GetCollection<CachedForecast>(_collectionName);
        }       
    }
}
