using System;

namespace PowerWeatherApp.Domain.Models
{
    public sealed class LocationInfo
    {
        public string City { get; set; }
        public string Country { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public DateTime LocalTime { get; set; }
    }
}
