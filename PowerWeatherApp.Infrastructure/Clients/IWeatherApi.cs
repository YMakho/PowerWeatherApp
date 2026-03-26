using System.Threading.Tasks;
using PowerWeatherApp.Infrastructure.Clients.Models;
using Refit;

namespace PowerWeatherApp.Infrastructure.Clients
{
    public interface IWeatherApi
    {
        [Get("/current.json")]
        Task<ApiResponse<WeatherApiRsponse>> RequestCurrentWeather(
            [AliasAs("key")] string apiKey,
            [AliasAs("q")] string coordinates);
       
        [Get("/forecast.json")]
        Task<ApiResponse<WeatherApiRsponse>> RequestForecast(
            [AliasAs("key")] string apiKey,
            [AliasAs("q")] string coordinates,
            [AliasAs("days")] int days);
    }
}
