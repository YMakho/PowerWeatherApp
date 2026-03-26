using PowerWeatherApp.Web.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace PowerWeatherApp.Web.Services
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public WeatherApiClient(HttpClient httpClient, JsonSerializerOptions jsonOptions)
        {
            _httpClient = httpClient;
            _jsonOptions = jsonOptions;
        }

        public async Task<WeatherResponseDto> GetCurrentWeather(double lat, double lon, CancellationToken cancellationToken = default)
        {
            var url = $"/api/weather/current?lat={lat}&lon={lon}";

            return await GetFromJsonAsync<WeatherResponseDto>(url, cancellationToken);
        }

        public async Task<WeatherResponseDto> GetForecast(double lat, double lon, int days = 3, CancellationToken cancellationToken = default)
        {
            var url = $"/api/weather/forecast?lat={lat}&lon={lon}&days={days}";

            return await GetFromJsonAsync<WeatherResponseDto>(url, cancellationToken);
        }

        private async Task<T?> GetFromJsonAsync<T>(string url, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    Console.WriteLine($"API Error: {response.StatusCode} - {errorContent}");

                    throw new HttpRequestException($"Request failed with status {response.StatusCode}");
                }

                return await response.Content.ReadFromJsonAsync<T>(_jsonOptions, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Network/Serialization Error: {ex.Message}");
                throw new ApplicationException("API error", ex);
            }
        }
    }

}

