using PowerWeatherApp.Application.DTOs;
using PowerWeatherApp.Domain.Interfaces;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace PowerWeatherApp.Application
{
    public sealed class WeatherService : IWeatherService
    {
        private readonly IWeatherGateway _weatherGateway;
        private readonly IWeatherRepository _repository;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public WeatherService(IWeatherGateway weatherGateway, IWeatherRepository repository)
        {
            _weatherGateway = weatherGateway ?? throw new ArgumentNullException(nameof(weatherGateway));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<WeatherResponseDto> GetCurrentWeather(
            double lat,
            double lon,
            CancellationToken cancellationToken = default)
        {
            var coordinates = $"{lat.ToString(CultureInfo.InvariantCulture)},{lon.ToString(CultureInfo.InvariantCulture)}";

            var apiResponse = await _weatherGateway.RequestCurrentWeather(coordinates, cancellationToken);

            return apiResponse;
        }

        public async Task<WeatherResponseDto> GetForecast(
            double lat,
            double lon,
            int days = 3,
            CancellationToken cancellationToken = default)
        {
            var coordinates = $"{lat.ToString(CultureInfo.InvariantCulture)},{lon.ToString(CultureInfo.InvariantCulture)}";

            var apiResponce = await _weatherGateway.RequestForecast(coordinates, days, cancellationToken);

            return apiResponce;
        }
    }
}
