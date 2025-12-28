using Refit;

namespace WeatherApp;

public interface IOpenWeatherForecastApi
{
    /// <summary>
    /// Gets the daily forecast of the location specified.
    /// </summary>
    /// <param name="latitude">The latitude for the forecast.</param>
    /// <param name="longitude">The longitude for the forecast.</param>
    /// <param name="apiKey">The service api key.</param>
    /// <param name="days">The days for the forecast to be calculated. Can be up to 16.</param>
    /// <param name="units">Units of measurement. standard, metric and imperial units are available.</param>
    /// <param name="language">Language code</param>
    /// <param name="mode">Data format. Possible values are: json, xml. If the mode parameter is empty the format is JSON by default</param>
    /// <returns></returns>
    [Get("/forecast/daily")]
    Task<DailyForecastResponse> GetDailyForecastAsync(
        [AliasAs("lat")] double latitude,
        [AliasAs("lon")] double longitude,
        [AliasAs("appid")] string apiKey,
        [AliasAs("cnt")] int? days = null,
        [AliasAs("units")] string? units = null,
        [AliasAs("lang")] string? language = null,
        [AliasAs("mode")] string? mode = null
    );
}

