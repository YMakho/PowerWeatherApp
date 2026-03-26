using Newtonsoft.Json;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class DayInfoApi
    {
        [JsonProperty("maxtemp_c")] public double MaxTempC { get; set; }
        [JsonProperty("mintemp_c")] public double MinTempC { get; set; }
        [JsonProperty("condition")] public ConditionApi Condition { get; set; }
        [JsonProperty("uv")] public double Uv { get; set; }
    }
}
