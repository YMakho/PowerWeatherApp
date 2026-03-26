using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PowerWeatherApp.Web.Services;
using System.Text.Json;

namespace PowerWeatherApp.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            
            
            builder.Services.AddHttpClient<IWeatherApiClient, WeatherApiClient>((sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseUrl = config["ApiSettings:BaseUrl"] ?? "https://localhost:44332/"; 
                client.BaseAddress = new Uri(baseUrl);
            });

            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });

            await builder.Build().RunAsync();
        }
    }
}
