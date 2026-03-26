namespace PowerWeatherApp.Web.Models
{
    public sealed class HourlyWeatherDto
    {
        public string Time { get; set; } 
        public double Temp { get; set; }
        public string IconUrl { get; set; }
        public int RainChance { get; set; }
    }
}
