using System;

namespace PowerWeatherApp.Domain.Models
{
    public sealed class CurrentWeather
    {
        public double TempC { get; set; }
        public double FeelsLikeC { get; set; }
        public string Condition { get; set; }
        public string IconUrl { get; set; }
        public double WindKph { get; set; }
        public int Humidity { get; set; }
        public bool IsDay { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
