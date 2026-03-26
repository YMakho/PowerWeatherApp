using System;

namespace PowerWeatherApp.Domain.Models
{
    public sealed class HourlyForecast
    {
        public DateTime Time { get; set; }
        public double TempC { get; set; }
        public string Condition { get; set; }
        public string IconUrl { get; set; }
        public int ChanceOfRain { get; set; }
    }
}
