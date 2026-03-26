using Newtonsoft.Json;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class ConditionApi
    {
        [JsonProperty("text")] public string Text { get; set; }
        [JsonProperty("icon")] public string Icon { get; set; } 
        [JsonProperty("code")] public int Code { get; set; }
    }
}
