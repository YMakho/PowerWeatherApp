using PowerWeatherApp.Domain.Models;
using PowerWeatherApp.Infrastructure.Clients.Models;
using System;
using System.Globalization;
using System.Linq;

namespace PowerWeatherApp.Infrastructure.Mappings
{
    public static class ToDomainMappingExtensions
    {
        public static WeatherForecast ToDomain(this WeatherApiRsponse response)
        {
            if (response == null) return null;

            return new WeatherForecast
            {
                Location = response.Location?.ToDomain(),
                Current = response.Current?.ToDomain(),
                
                DailyForecasts = response.Forecast?.Forecastday?
                    .Take(3)
                    .Select(d => d.ToDomain())
                    .ToList(),

                HourlyForecasts = response.Forecast?.Forecastday?
                    .SelectMany(d => d.Hour.Select(h => h.ToDomain()))
                    .ToList(),

                CachedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };
        }

        public static LocationInfo ToDomain(this LocationApi loc) =>
            new LocationInfo
            {
                City = loc.Name,
                Country = loc.Country,
                Lat = loc.Lat,
                Lon = loc.Lon,
                LocalTime = DateTime.TryParse(loc.LocalTime, out var dt) 
                ? dt : DateTime.UtcNow
            };

        public static CurrentWeather ToDomain(this CurrentWeatherApi curr) =>
            new CurrentWeather
            {
                TempC = curr.TempC,
                FeelsLikeC = curr.FeelsLikeC,
                Condition = curr.Condition?.Text,
                IconUrl = curr.Condition?.Icon,
                WindKph = curr.WindKph,
                Humidity = curr.Humidity,
                IsDay = curr.IsDay == 1,
                LastUpdated = DateTime.TryParse(curr.LastUpdated, out var dt) 
                ? dt : DateTime.UtcNow
            };

        public static DailyForecast ToDomain(this ForecastDayApi day) =>
            new DailyForecast
            {
                Date = DateTime.TryParseExact(
                    day.Date, 
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var dt) 
                ? dt : DateTime.MinValue,
                MaxTempC = day.Day?.MaxTempC ?? 0,
                MinTempC = day.Day?.MinTempC ?? 0,
                Condition = day.Day?.Condition?.Text,
                IconUrl = day.Day?.Condition?.Icon
            };

        public static HourlyForecast ToDomain(this HourInfoApi hour)
        {
            DateTime time = DateTime.ParseExact(hour.Time, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

            return new HourlyForecast
            {
                Time = time,
                TempC = hour.TempC,
                Condition = hour.Condition?.Text,
                IconUrl = hour.Condition?.Icon,
                ChanceOfRain = hour.ChanceOfRain
            };
        }
    }
}
