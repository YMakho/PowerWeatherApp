using System;

namespace PowerWeatherApp.Domain.Models
{
    public sealed class DailyForecast
    {
        public DateTime Date { get; set; }
        public double MaxTempC { get; set; }
        public double MinTempC { get; set; }
        public string Condition { get; set; }
        public string IconUrl { get; set; }
    }
}
