using PowerWeatherApp.Application.DTOs;
using PowerWeatherApp.Domain.Models;
using System;
using System.Globalization;
using System.Linq;

namespace PowerWeatherApp.Infrastructure.Mappings
{
    public static class ToDtoMappingExtensions
    {
        public static WeatherResponseDto ToDto(this WeatherForecast domain, bool isFromCache = false)
        {
            if (domain == null) return null;

            DateTime locationNow = domain.Location?.LocalTime != default
            ? domain.Location.LocalTime
            : DateTime.UtcNow;

            var todayHours = domain.HourlyForecasts
                .Where(h => h.Time > locationNow && h.Time.Date == locationNow.Date)
                .ToList();

            var tomorrowHours = domain.HourlyForecasts
                .Where(h => h.Time.Date == locationNow.AddDays(1).Date)
                .ToList();

            var hourlyDto = todayHours.Concat(tomorrowHours)
                .Select(h => new HourlyWeatherDto
                {
                    Time = h.Time.ToString("HH:mm"),
                    Temp = h.TempC,
                    IconUrl = h.IconUrl,
                    RainChance = h.ChanceOfRain
                })
                .ToList();

            return new WeatherResponseDto
            {
                Location = new LocationDto
                {
                    City = domain.Location?.City,
                    LocalTime = domain.Location?.LocalTime.ToString("HH:mm")
                },
                Current = new CurrentWeatherDto
                {
                    Temp = domain.Current?.TempC ?? 0,
                    FeelsLike = domain.Current?.FeelsLikeC ?? 0,
                    Condition = domain.Current?.Condition,
                    IconUrl = domain.Current?.IconUrl,
                    WindSpeed = domain.Current?.WindKph ?? 0,
                    Humidity = domain.Current?.Humidity ?? 0
                },
                Daily = domain.DailyForecasts?.Select(d => new DailyWeatherDto
                {
                    Date = d.Date.ToString("dd MMM", CultureInfo.InvariantCulture),
                    MaxTemp = d.MaxTempC,
                    MinTemp = d.MinTempC,
                    Condition = d.Condition,
                    IconUrl = d.IconUrl
                })
                .ToList(),
                Hourly = hourlyDto,
                IsFromCache = true,//(DateTime.UtcNow - domain.CachedAt).TotalMinutes > 1,
                LastUpdated = domain.CachedAt
            };
        }
    }
}

