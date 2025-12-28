using Microsoft.Maui.Devices.Sensors;
using Refit;
using WeatherApp.API.DTOs.GeocodingDTO;

namespace WeatherApp;

public interface IGeoCodingApi
{
    [Get("/geo/1.0/direct")]
    Task<List<GeoLocation>> GetCityCoordinatesAsync(
        [AliasAs("q")] string cityName,
        [AliasAs("limit")] int limit = 5,
        [AliasAs("appid")] string apiKey = "911d53ba5fad2df0d2632608585653a2"
    );
}
