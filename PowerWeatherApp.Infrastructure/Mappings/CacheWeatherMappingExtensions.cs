using Newtonsoft.Json;
using PowerWeatherApp.Domain.Models;
using PowerWeatherApp.Infrastructure.Clients.Models;
using PowerWeatherApp.Infrastructure.Persistence.MongoEntities;
using System;
using System.Globalization;
using System.Linq;

namespace PowerWeatherApp.Infrastructure.Mappings
{
    public static class CacheWeatherMappingExtensions
    {
        public static WeatherForecast ToDomain(this CachedForecast entity)
        {
            if (entity == null) return null;

            if (string.IsNullOrEmpty(entity.RawJson))
                return null;

            try
            {
                var apiResponse = JsonConvert.DeserializeObject<WeatherApiRsponse>(entity.RawJson);

                if (apiResponse == null) return null;

                var domainModel = apiResponse.ToDomain();

                domainModel.CachedAt = entity.CachedAt;
                domainModel.ExpiresAt = entity.ExpiresAt;

                return domainModel;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing cached forecast: {ex.Message}");
                return null;
            }
        }
        public static CachedForecast ToEntity(this WeatherForecast domain)
        {
            if (domain == null) return null;

            var coordinatesKey = $"{domain.Location.Lat.ToString(CultureInfo.InvariantCulture)},{domain.Location.Lon.ToString(CultureInfo.InvariantCulture)}";           

            var apiResponse = domain.ToApiResponse();

            string rawJson = JsonConvert.SerializeObject(apiResponse);

            return new CachedForecast
            {
                Coordinates = coordinatesKey.ToLowerInvariant(),
                Location = domain.Location?.City,
                RawJson = rawJson,
                CachedAt = domain.CachedAt,
                ExpiresAt = domain.ExpiresAt,
            };
        }

        public static WeatherApiRsponse ToApiResponse(this WeatherForecast domain)
        {
            if (domain == null) return null;

            var response = new WeatherApiRsponse
            {
                Location = new LocationApi
                {
                    Name = domain.Location.City,
                    Country = domain.Location.Country,
                    Lat = domain.Location.Lat,
                    Lon = domain.Location.Lon,
                    LocalTime = domain.Location.LocalTime.ToString("yyyy-MM-dd HH:mm")
                },
                Current = new CurrentWeatherApi
                {
                    TempC = domain.Current.TempC,
                    FeelsLikeC = domain.Current.FeelsLikeC,
                    WindKph = domain.Current.WindKph,
                    Humidity = domain.Current.Humidity,
                    IsDay = domain.Current.IsDay ? 1 : 0,
                    Condition = new ConditionApi { Text = domain.Current.Condition, Icon = domain.Current.IconUrl }
                },
                Forecast = new ForecastApi { Forecastday = new System.Collections.Generic.List<ForecastDayApi>() }
            };

            if (domain.DailyForecasts != null)
            {
                foreach (var day in domain.DailyForecasts)
                {
                    var dayApi = new ForecastDayApi
                    {
                        Date = day.Date.ToString("yyyy-MM-dd"),
                        Day = new DayInfoApi
                        {
                            MaxTempC = day.MaxTempC,
                            MinTempC = day.MinTempC,
                            Condition = new ConditionApi { Text = day.Condition, Icon = day.IconUrl }
                        },
                        Hour = new System.Collections.Generic.List<HourInfoApi>()
                    };

                    var hours = domain.HourlyForecasts?.Where(h => h.Time.Date == day.Date.Date);
                    if (hours != null)
                    {
                        foreach (var h in hours)
                        {
                            dayApi.Hour.Add(new HourInfoApi
                            {
                                Time = h.Time.ToString("yyyy-MM-dd HH:mm"),
                                TempC = h.TempC,
                                ChanceOfRain = h.ChanceOfRain,
                                Condition = new ConditionApi { Text = h.Condition, Icon = h.IconUrl }
                            });
                        }
                    }

                    response.Forecast.Forecastday.Add(dayApi);
                }
            }

            return response;
        }
    }
}
