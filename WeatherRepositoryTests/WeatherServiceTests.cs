using Moq;
using PowerWeatherApp.Application;
using PowerWeatherApp.Application.DTOs;
using PowerWeatherApp.Domain.Interfaces;

namespace WeatherRepositoryTests
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherGateway> _mockGateway;
        private readonly Mock<IWeatherRepository> _mockRepository;
        private readonly WeatherService _service;

        public WeatherServiceTests()
        {
            _mockGateway = new Mock<IWeatherGateway>();
            _mockRepository = new Mock<IWeatherRepository>();
            _service = new WeatherService(_mockGateway.Object, _mockRepository.Object);
        }

        [Fact]
        public async Task GetCurrentWeather_ShouldFormatCoordinatesAndCallGateway()
        {
            // Arrange
            double lat = 55.75;
            double lon = 37.61;

            string expectedCoordinates = "55.75,37.61";

            var expectedDto = new WeatherResponseDto
            {
                Location = new LocationDto { City = "Moscow" }
            };

            _mockGateway.Setup(x => x.RequestCurrentWeather(
                    expectedCoordinates,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _service.GetCurrentWeather(lat, lon);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Moscow", result.Location.City);

            _mockGateway.Verify(x => x.RequestCurrentWeather(
                expectedCoordinates,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetForecast_ShouldPassDaysParameterToGateway()
        {
            // Arrange
            double lat = 55.75;
            double lon = 37.61;
            int days = 5;

            string expectedCoordinates = "55.75,37.61";

            var expectedDto = new WeatherResponseDto
            {
                Location = new LocationDto { City = "Moscow" }
            };

            _mockGateway.Setup(x => x.RequestForecast(
                    expectedCoordinates,
                    days,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedDto);

            // Act
            var result = await _service.GetForecast(lat, lon, days);

            // Assert
            Assert.NotNull(result);

            _mockGateway.Verify(x => x.RequestForecast(
                expectedCoordinates,
                days,
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetCurrentWeather_ShouldUseInvariantCultureForFormatting()
        {
            // Arrange
            double lat = 55.75;
            double lon = 37.61;

            // Act
            await _service.GetCurrentWeather(lat, lon);

            // Assert
            _mockGateway.Verify(x => x.RequestCurrentWeather(
                  It.Is<string>(s =>
                      s.Contains(".") && 
                      s.IndexOf(',') == s.LastIndexOf(',') // Запятая встречается только один раз
                  ),
                  It.IsAny<CancellationToken>()),
                  Times.Once);
        }
    }
}
