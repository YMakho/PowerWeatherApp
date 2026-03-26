using PowerWeatherApp.Application.DTOs;
using PowerWeatherApp.Infrastructure.Clients.Models;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace PowerWeatherApp.Infrastructure.Mappings
{
    public static class WeatherApiResponseToDto
    {
        public static WeatherResponseDto ToDto(this WeatherApiRsponse response, bool isFromCache = false)
        {
            if (response == null) return null;

            DateTime locationTime = DateTime.TryParse(response.Location?.LocalTime, out var lt)
                ? lt
                : DateTime.UtcNow;

            var hourlyDto = new List<HourlyWeatherDto>();
            var dailyDto = new List<DailyWeatherDto>();

            if (response.Forecast?.Forecastday != null)
            {
                foreach (var day in response.Forecast.Forecastday)
                {
                    dailyDto.Add(new DailyWeatherDto
                    {
                        Date = FormatDate(day.Date), 
                        MaxTemp = day.Day?.MaxTempC ?? 0,
                        MinTemp = day.Day?.MinTempC ?? 0,
                        Condition = day.Day?.Condition?.Text,
                        IconUrl = day.Day?.Condition?.Icon
                    });

                    if (day.Hour != null)
                    {
                        foreach (var hour in day.Hour)
                        {
                            if (DateTime.TryParseExact(hour.Time, "yyyy-MM-dd HH:mm",
                                CultureInfo.InvariantCulture, DateTimeStyles.None, out var hourTime))
                            {
                                bool isToday = hourTime.Date == locationTime.Date;
                                bool isTomorrow = hourTime.Date == locationTime.Date.AddDays(1);

                                if ((isToday && hourTime > locationTime) || isTomorrow)
                                {
                                    hourlyDto.Add(new HourlyWeatherDto
                                    {
                                        Time = hourTime.ToString("HH:mm"), 
                                        Temp = hour.TempC,
                                        IconUrl = hour.Condition?.Icon,
                                        RainChance = hour.ChanceOfRain
                                    });
                                }
                            }
                        }
                    }
                }
            }

            return new WeatherResponseDto
            {
                Location = new LocationDto
                {
                    City = response.Location?.Name,
                    LocalTime = locationTime.ToString("HH:mm")
                },
                Current = new CurrentWeatherDto
                {
                    Temp = response.Current?.TempC ?? 0,
                    FeelsLike = response.Current?.FeelsLikeC ?? 0,
                    Condition = response.Current?.Condition?.Text,
                    IconUrl = response.Current?.Condition?.Icon,
                    WindSpeed = response.Current?.WindKph ?? 0,
                    Humidity = response.Current?.Humidity ?? 0
                },
                Daily = dailyDto,
                Hourly = hourlyDto,
                IsFromCache = isFromCache,
                LastUpdated = DateTime.UtcNow 
            };
        }

        private static string FormatDate(string dateStr)
        {
            if (DateTime.TryParseExact(dateStr, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date.ToString("dd MMM", CultureInfo.InvariantCulture); 
            }
            return dateStr;
        }
    }
}
