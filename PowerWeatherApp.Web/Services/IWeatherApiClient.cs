using PowerWeatherApp.Web.Models;

namespace PowerWeatherApp.Web.Services
{
    public interface IWeatherApiClient
    {
        Task<WeatherResponseDto> GetCurrentWeather(double lat, double lon, CancellationToken cancellationToken = default);
        Task<WeatherResponseDto> GetForecast(double lat, double lon, int days = 3, CancellationToken cancellationToken = default);
    }
}
