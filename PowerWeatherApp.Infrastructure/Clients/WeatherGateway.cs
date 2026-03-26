using Microsoft.Extensions.Logging;
using PowerWeatherApp.Application;
using PowerWeatherApp.Application.DTOs;
using PowerWeatherApp.Domain.Interfaces;
using PowerWeatherApp.Infrastructure.Mappings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PowerWeatherApp.Infrastructure.Clients
{
    public sealed class WeatherGateway : IWeatherGateway
    {
        private readonly IWeatherApi _weatherApi;
        private readonly IWeatherRepository _repository;
        private readonly ILogger<WeatherGateway> _logger;
        private readonly string _apiKey;
        public WeatherGateway(
            IWeatherApi weatherApi, 
            IWeatherRepository repository, 
            ILogger<WeatherGateway> logger, 
            string apiKey)
        {
            _repository = repository;
            _weatherApi = weatherApi ?? throw new ArgumentNullException(nameof(weatherApi));
            _logger = logger;
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<WeatherResponseDto> RequestCurrentWeather(string coordinates, CancellationToken cancellationToken = default)
        {
            bool isDbAvaliable = await _repository.IsDatabaseAvailableAsync(cancellationToken);            

            if (isDbAvaliable)
            {
                var cachedData = await _repository.GetAsync(coordinates, cancellationToken);

                if (cachedData != null && !cachedData.IsExpired())
                {
                    return cachedData.ToDto(isFromCache: true);
                }
            }

            try
            {
                var apiResponse = await _weatherApi.RequestCurrentWeather(_apiKey, coordinates);

                if (!apiResponse.IsSuccessStatusCode || apiResponse.Content == null)
                {
                    _logger.LogWarning($"API Error: {apiResponse.StatusCode}");
                    throw new Exception($"API Error: {apiResponse.StatusCode}");
                }

                var newCacheEntry = apiResponse.Content.ToDomain();
                await _repository.SaveAsync(newCacheEntry, cancellationToken);

                _logger.LogInformation("[DB] API Response saved");

                return apiResponse.Content.ToDto(isFromCache: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[WeatherGateway/RequestCurrentWeather] Error: {ex.Message}");
                throw;
            }
        }

        public async Task<WeatherResponseDto> RequestForecast(string coordinates, int days = 3, CancellationToken cancellationToken = default)
        {
            bool isDbAvaliable = await _repository.IsDatabaseAvailableAsync(cancellationToken);

            if (isDbAvaliable)
            {
                var cachedData = await _repository.GetAsync(coordinates, cancellationToken);

                if (cachedData != null && !cachedData.IsExpired())
                {                   
                    return cachedData.ToDto(isFromCache: true);
                }
            }

            try
            {
                var apiResponse = await _weatherApi.RequestForecast(_apiKey, coordinates, days);

                if (!apiResponse.IsSuccessStatusCode || apiResponse.Content == null)
                {
                    _logger.LogWarning($"API Error: {apiResponse.StatusCode}");
                    throw new Exception($"Ошибка API: {apiResponse.StatusCode}");
                }

                var newCacheEntry = apiResponse.Content.ToDomain();
                await _repository.SaveAsync(newCacheEntry, cancellationToken);

                _logger.LogInformation("[DB] API Response saved");

                return apiResponse.Content.ToDto(isFromCache: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"[WeatherGateway/RequestForecast] Error: {ex.Message}");
                throw;
            }
        }
    }
}
