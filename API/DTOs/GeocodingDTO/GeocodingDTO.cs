using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherApp.API.DTOs.GeocodingDTO;

public class GeoLocation
{
    public string Name { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? State { get; set; }
}

