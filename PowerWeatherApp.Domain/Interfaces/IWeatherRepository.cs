using PowerWeatherApp.Domain.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PowerWeatherApp.Domain.Interfaces
{
    public interface IWeatherRepository
    {
        Task<WeatherForecast> GetAsync(string coordinates, CancellationToken cancellationToken = default);
        Task SaveAsync(WeatherForecast forecast, CancellationToken cancellationToken = default);
        Task<bool> IsDatabaseAvailableAsync(CancellationToken cancellationToken = default);
    }
}
