using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using PowerWeatherApp.Domain.Interfaces;
using PowerWeatherApp.Domain.Models;
using PowerWeatherApp.Infrastructure.Mappings;
using PowerWeatherApp.Infrastructure.Persistence.Interface;
using PowerWeatherApp.Infrastructure.Persistence.MongoEntities;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace PowerWeatherApp.Infrastructure.Repository
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly IMongoDbContext _collectionService;
        private readonly ILogger<WeatherRepository> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public WeatherRepository(IMongoDbContext collectionService, ILogger<WeatherRepository> logger)
        {
            _collectionService = collectionService;
            _logger = logger;
        }

        public async Task<WeatherForecast> GetAsync(
            string coordinates, 
            CancellationToken cancellationToken = default)
        {
            var normalizedCoordinates = coordinates.ToLowerInvariant();

            var filter = Builders<CachedForecast>.Filter.Eq(x => x.Coordinates, normalizedCoordinates);

            var entity = await _collectionService.GetWeatherCollection()
                .Find(filter)
                .SortByDescending(x => x.CachedAt)
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                _logger.LogInformation("[DB] Cached data not found");
                return null;
            }

            _logger.LogInformation("[DB] Cached data was found");

            var domainModel = entity.ToDomain();

            if (domainModel.IsExpired())
            {
                _logger.LogInformation("[DB] Cached data is expired");
                return null;
            }

            _logger.LogInformation("[DB] Returned cached data");
            return domainModel;
        }

        public async Task SaveAsync(
            WeatherForecast forecast, 
            CancellationToken cancellationToken = default)
        {
            var entity = forecast.ToEntity();

            var locationKey = $@"{forecast.Location.Lat.ToString(CultureInfo.InvariantCulture)},{forecast.Location.Lon.ToString(CultureInfo.InvariantCulture)}";

            entity.Coordinates = locationKey.ToLowerInvariant();
            entity.Location = forecast.Location.City.ToLowerInvariant();
            entity.CachedAt = DateTime.UtcNow;
            entity.ExpiresAt = forecast.CachedAt + _cacheDuration;

            var filter = Builders<CachedForecast>.Filter.Eq(x => x.Coordinates, entity.Coordinates);

            var update = Builders<CachedForecast>.Update
                .Set(x => x.Coordinates, entity.Coordinates)
                .Set(x => x.RawJson, entity.RawJson)
                .Set(x => x.CachedAt, entity.CachedAt)
                .Set(x => x.ExpiresAt, entity.ExpiresAt)
                .Set(x => x.Location, entity.Location); ;
            try
            {
                await _collectionService.GetWeatherCollection()
                .UpdateOneAsync(
                filter,
                update,
                new UpdateOptions { IsUpsert = true },
                cancellationToken);

                _logger.LogInformation("[DB] Data was saved");
            }
            catch (Exception ex)
            {
                _logger.LogError($"[DB] Error while save data: {ex.Message}");
                throw;
            }
            
        }

        public async Task<bool> IsDatabaseAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {               
                await _collectionService.Database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                _logger.LogInformation("[DB] Connection established");
                return true;
            }
            catch
            {
                _logger.LogWarning("[DB] Connection not established");
                return false;
            }
        }
    }
}

