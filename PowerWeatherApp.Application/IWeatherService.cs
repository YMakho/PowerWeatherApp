using PowerWeatherApp.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace PowerWeatherApp.Application
{
    public interface IWeatherService
    {
        Task<WeatherResponseDto> GetCurrentWeather(double lat, double lon, CancellationToken cancellationToken = default);
        Task<WeatherResponseDto> GetForecast(double lat, double lon, int days = 3, CancellationToken cancellationToken = default);
    }
}
