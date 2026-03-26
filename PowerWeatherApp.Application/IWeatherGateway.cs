using PowerWeatherApp.Application.DTOs;
using System.Threading;
using System.Threading.Tasks;

namespace PowerWeatherApp.Application
{
    public interface IWeatherGateway
    {
        Task<WeatherResponseDto> RequestCurrentWeather(string coordinates, CancellationToken cancellationToken = default);
        Task<WeatherResponseDto> RequestForecast(string coordinates, int days = 3, CancellationToken cancellationToken = default);
    }
}
