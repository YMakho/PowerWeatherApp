using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using PowerWeatherApp.Infrastructure.Persistence.Interface;
using PowerWeatherApp.Infrastructure.Repository;

namespace WeatherRepositoryTests
{
    public class WeatherRepositoryTests
    {
        private readonly Mock<IMongoDbContext> _mockDbContext;
        private readonly Mock<ILogger<WeatherRepository>> _mockLogger;
        private readonly WeatherRepository _repository;

        public WeatherRepositoryTests()
        {
            _mockDbContext = new Mock<IMongoDbContext>();
            _mockLogger = new Mock<ILogger<WeatherRepository>>();

            var mockDatabase = new Mock<IMongoDatabase>();
            _mockDbContext.Setup(x => x.Database).Returns(mockDatabase.Object);

            _repository = new WeatherRepository(_mockDbContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task IsDatabaseAvailableAsync_WhenPingSuccess_ReturnsTrue()
        {
            // Arrange
            _mockDbContext.Setup(x => x.Database.RunCommandAsync(
                It.IsAny<Command<BsonDocument>>(),
                It.IsAny<ReadPreference>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BsonDocument("ok", 1));

            // Act
            var result = await _repository.IsDatabaseAvailableAsync();

            // Assert
            Assert.True(result);
        }       
    }
}

