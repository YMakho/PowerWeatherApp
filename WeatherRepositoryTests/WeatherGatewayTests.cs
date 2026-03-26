using Microsoft.Extensions.Logging;
using Moq;
using PowerWeatherApp.Domain.Interfaces;
using PowerWeatherApp.Domain.Models;
using PowerWeatherApp.Infrastructure.Clients;
using PowerWeatherApp.Infrastructure.Clients.Models;
using Refit;
using System.Net;

namespace WeatherRepositoryTests
{
    public class WeatherGatewayTests
    {
        private readonly Mock<IWeatherApi> _mockWeatherApi;
        private readonly Mock<IWeatherRepository> _mockRepository;
        private readonly Mock<ILogger<WeatherGateway>> _mockLogger;
        private readonly WeatherGateway _gateway;
        private readonly string _testApiKey = "test_api_key";

        public WeatherGatewayTests()
        {
            _mockWeatherApi = new Mock<IWeatherApi>();
            _mockRepository = new Mock<IWeatherRepository>();
            _mockLogger = new Mock<ILogger<WeatherGateway>>();

            _gateway = new WeatherGateway(
                _mockWeatherApi.Object,
                _mockRepository.Object,
                _mockLogger.Object,
                _testApiKey
            );
        }

        [Fact]
        public async Task RequestCurrentWeather_CacheHasValidData_ReturnsFromCache()
        {
            // Arrange
            var coordinates = "55.75,37.61";

            var cachedData = new WeatherForecast
            {
                Location = new LocationInfo { City = "Moscow" },
                CachedAt = DateTime.UtcNow.AddMinutes(-5),
                ExpiresAt = DateTime.UtcNow.AddMinutes(25) 
            };

            _mockRepository.Setup(x => x.IsDatabaseAvailableAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetAsync(coordinates, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedData);

            // Act
            var result = await _gateway.RequestCurrentWeather(coordinates);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsFromCache); 

            _mockWeatherApi.Verify(x => x.RequestCurrentWeather(
                It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            _mockRepository.Verify(x => x.SaveAsync(
                It.IsAny<WeatherForecast>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task RequestCurrentWeather_CacheIsEmpty_CallsApiAndSaves()
        {
            // Arrange
            var coordinates = "55.75,37.61";

            _mockRepository.Setup(x => x.IsDatabaseAvailableAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetAsync(coordinates, It.IsAny<CancellationToken>()))
                .ReturnsAsync((WeatherForecast)null);

            var apiContent = new WeatherApiRsponse
            {
                Location = new LocationApi { Name = "Moscow" }
            };

            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var apiResponse = new ApiResponse<WeatherApiRsponse>(httpResponse, apiContent, null);

            _mockWeatherApi.Setup(x => x.RequestCurrentWeather(_testApiKey, coordinates))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _gateway.RequestCurrentWeather(coordinates);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsFromCache); 

            _mockRepository.Verify(x => x.SaveAsync(
                It.Is<WeatherForecast>(f => f.Location.City == "Moscow"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RequestCurrentWeather_ApiFails_ThrowsException()
        {
            // Arrange
            var coordinates = "55.75,37.61";

            _mockRepository.Setup(x => x.IsDatabaseAvailableAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); 

            var httpResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var apiResponse = new ApiResponse<WeatherApiRsponse>(httpResponse, null, null);

            _mockWeatherApi.Setup(x => x.RequestCurrentWeather(_testApiKey, coordinates))
                .ReturnsAsync(apiResponse);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _gateway.RequestCurrentWeather(coordinates));
        }

        [Fact]
        public async Task RequestForecast_CacheExpired_CallsApi()
        {
            // Arrange
            var coordinates = "55.75,37.61";
            var days = 3;

            var expiredCache = new WeatherForecast
            {
                Location = new LocationInfo { City = "Old Data" },
                CachedAt = DateTime.UtcNow.AddHours(-1),
                ExpiresAt = DateTime.UtcNow.AddMinutes(-10) 
            };

            _mockRepository.Setup(x => x.IsDatabaseAvailableAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository.Setup(x => x.GetAsync(coordinates, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expiredCache);

            // Мокаем API
            var apiContent = new WeatherApiRsponse { Location = new LocationApi { Name = "New Data" } };
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
            var apiResponse = new ApiResponse<WeatherApiRsponse>(httpResponse, apiContent, null);

            _mockWeatherApi.Setup(x => x.RequestForecast(_testApiKey, coordinates, days))
                .ReturnsAsync(apiResponse);

            // Act
            var result = await _gateway.RequestForecast(coordinates, days);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Data", result.Location.City); 

            _mockWeatherApi.Verify(x => x.RequestForecast(
                _testApiKey, coordinates, days), Times.Once);
        }
    }
}
