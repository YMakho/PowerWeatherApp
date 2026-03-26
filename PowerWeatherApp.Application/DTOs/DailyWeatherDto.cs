namespace PowerWeatherApp.Application.DTOs
{
    public sealed class DailyWeatherDto
    {
        public string Date { get; set; } 
        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }
        public string Condition { get; set; }
        public string IconUrl { get; set; }
    }
}
