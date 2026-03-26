using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PowerWeatherApp.Infrastructure.Clients;
using Refit;

namespace WeatherRepositoryTests
{
    public class WeatherApiIntegrationTests
    {
        private readonly IWeatherApi _weatherApi;

        private const string ApiKey = "fa8b3df74d4042b9aa7135114252304";

        public WeatherApiIntegrationTests()
        {
            var jsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var refitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(jsonSettings)
            };

            _weatherApi = RestService.For<IWeatherApi>("http://api.weatherapi.com/v1", refitSettings);
        }

        [Fact]
        public async Task RequestCurrentWeather_RealApi_ReturnsSuccess()
        {
            // Arrange
            var coordinates = "55.75,37.61";

            // Act
            var response = await _weatherApi.RequestCurrentWeather(ApiKey, coordinates);

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"API вернул ошибку: {response.StatusCode}");

            Assert.NotNull(response.Content);

            Assert.NotNull(response.Content.Location);
            Assert.False(string.IsNullOrEmpty(response.Content.Location.Name));

            Assert.NotNull(response.Content.Current);
            Assert.True(response.Content.Current.TempC > -50 && response.Content.Current.TempC < 50);
        }

        [Fact]
        public async Task RequestForecast_RealApi_ReturnsThreeDays()
        {
            // Arrange
            var coordinates = "55.75,37.61";
            var days = 3;

            // Act
            var response = await _weatherApi.RequestForecast(ApiKey, coordinates, days);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content?.Forecast?.Forecastday);

            Assert.Equal(days, response.Content.Forecast.Forecastday.Count);
        }
    }
}
