using System;
using System.Collections.Generic;

namespace PowerWeatherApp.Application.DTOs
{
    public sealed class WeatherResponseDto
    {
        public CurrentWeatherDto Current { get; set; }
        public List<HourlyWeatherDto> Hourly { get; set; }
        public List<DailyWeatherDto> Daily { get; set; }
        public LocationDto Location { get; set; }
        public bool IsFromCache { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
