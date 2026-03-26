using Newtonsoft.Json;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class HourInfoApi
    {
        [JsonProperty("time")] public string Time { get; set; } 
        [JsonProperty("temp_c")] public double TempC { get; set; }
        [JsonProperty("condition")] public ConditionApi Condition { get; set; }
        [JsonProperty("chance_of_rain")] public int ChanceOfRain { get; set; }
        [JsonProperty("will_it_rain")] public int WillItRain { get; set; }
    }
}
