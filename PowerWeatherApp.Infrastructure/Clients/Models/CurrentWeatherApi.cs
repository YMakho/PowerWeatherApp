using Newtonsoft.Json;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class CurrentWeatherApi
    {
        [JsonProperty("last_updated")] public string LastUpdated { get; set; }
        [JsonProperty("temp_c")] public double TempC { get; set; }
        [JsonProperty("condition")] public ConditionApi Condition { get; set; }
        [JsonProperty("wind_kph")] public double WindKph { get; set; }
        [JsonProperty("humidity")] public int Humidity { get; set; }
        [JsonProperty("feelslike_c")] public double FeelsLikeC { get; set; }
        [JsonProperty("is_day")] public int IsDay { get; set; }
    }
}
