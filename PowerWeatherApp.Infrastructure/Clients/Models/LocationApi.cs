using Newtonsoft.Json;

namespace PowerWeatherApp.Infrastructure.Clients.Models
{
    public sealed class LocationApi
    {
        [JsonProperty("name")] public string Name { get; set; } 
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("lat")] public double Lat { get; set; }
        [JsonProperty("lon")] public double Lon { get; set; }
        [JsonProperty("localtime")] public string LocalTime { get; set; }
    }
}
