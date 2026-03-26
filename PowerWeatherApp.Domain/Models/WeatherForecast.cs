using System;
using System.Collections.Generic;

namespace PowerWeatherApp.Domain.Models
{
    public sealed class WeatherForecast
    {
        public LocationInfo Location { get; set; }
        public CurrentWeather Current { get; set; }
        public List<DailyForecast> DailyForecasts { get; set; } = new List<DailyForecast>();
        public List<HourlyForecast> HourlyForecasts { get; set; } = new List<HourlyForecast>();
        public DateTime CachedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public bool IsExpired()
        {
            return ExpiresAt < DateTime.UtcNow;
        }
    }
}
