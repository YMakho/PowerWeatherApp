using Newtonsoft.Json;
using System.Collections.Generic;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class ForecastDayApi
    {
        [JsonProperty("date")] public string Date { get; set; } 
        [JsonProperty("day")] public DayInfoApi Day { get; set; }
        [JsonProperty("hour")] public List<HourInfoApi> Hour { get; set; }
    }
}
