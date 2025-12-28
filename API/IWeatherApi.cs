using Microsoft.Maui.Devices.Sensors;
using Refit;

namespace WeatherApp;

public interface IOpenWeatherForecastApi
{
    /// <summary>
    /// Gets the daily forecast of the location specified.
    /// </summary>
    /// <param name="lat">The latitude for the forecast.</param>
    /// <param name="lon">The longitude for the forecast.</param>
    /// <param name="appid">The service api key.</param>
    /// <param name="units">Units of measurement. standard, metric and imperial units are available.</param>
    /// <returns>The list of the forecats of 5 days</returns>
    [Get("/forecast")]
    Task<ForecastResponse> GetForecastAsync(
        [Query] double lat,
        [Query] double lon,
        [Query] string appid,
        [Query] string units = "metric");
}

