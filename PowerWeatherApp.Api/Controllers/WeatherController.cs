using PowerWeatherApp.Application;
using PowerWeatherApp.Application.DTOs;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace PowerWeatherApp.Api.Controllers
{
    [RoutePrefix("api/weather")]
    public class WeatherController : ApiController
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
        }

        [HttpGet]
        [Route("current")]
        public async Task<WeatherResponseDto> GetCurrentWeather(
            [FromUri] double lat = 55.752,//55.7558,
            [FromUri] double lon = 37.616,//37.6173,
            [FromUri] bool force = false)
        {
            var currentWeather = await _weatherService.GetCurrentWeather(lat, lon);
            return currentWeather;
        }
        [HttpGet]
        [Route("forecast")]
        public async Task<WeatherResponseDto> GetForecast(
            [FromUri] double lat = 55.752,
            [FromUri] double lon = 37.616,
            [FromUri] int days = 3,
            [FromUri] bool force = false)
        {
            var forecastWeather = await _weatherService.GetForecast(lat, lon, days);
            return forecastWeather;
        }
    }
}