using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class ForecastApi
    {
        [JsonProperty("forecastday")]
        public List<ForecastDayApi> Forecastday { get; set; }
    }
}
