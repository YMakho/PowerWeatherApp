using Newtonsoft.Json;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class WeatherApiRsponse
    {
        [JsonProperty("location")]
        public LocationApi Location { get; set; }

        [JsonProperty("current")]
        public CurrentWeatherApi Current { get; set; }

        [JsonProperty("forecast")]
        public ForecastApi Forecast { get; set; }
    }
}
