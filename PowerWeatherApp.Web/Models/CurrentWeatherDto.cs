namespace PowerWeatherApp.Web.Models
{
    public sealed class CurrentWeatherDto
    {
        public double Temp { get; set; }
        public double FeelsLike { get; set; }
        public string Condition { get; set; }
        public string IconUrl { get; set; }
        public double WindSpeed { get; set; }
        public int Humidity { get; set; }
    }
}
