
namespace WeatherApp;

/// <summary>
/// Root response dto for the api call that returns the daily weather forecast.
/// </summary>
public class DailyForecastResponse
{
    public City City { get; set; } = default!;
    public string Cod { get; set; } = string.Empty;
    public double Message { get; set; }
    public int Cnt { get; set; }
    public List<DailyForecast> List { get; set; } = new();
}

// Nested DTOs

public class City
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Coordinates Coord { get; set; } = default!;
    public string Country { get; set; } = string.Empty;
    public int Population { get; set; }
    public int Timezone { get; set; }
}

public class Coordinates
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}

public class DailyForecast
{
    public long Dt { get; set; }
    public long Sunrise { get; set; }
    public long Sunset { get; set; }

    public Temperature Temp { get; set; } = default!;
    public FeelsLike Feels_Like { get; set; } = default!;

    public int Pressure { get; set; }
    public int Humidity { get; set; }

    public List<WeatherCondition> Weather { get; set; } = new();

    public double Speed { get; set; }
    public int Deg { get; set; }
    public double Gust { get; set; }

    public int Clouds { get; set; }
    public double Pop { get; set; }

    public double? Rain { get; set; }
    public double? Snow { get; set; }
}

public class Temperature
{
    public double Day { get; set; }
    public double Min { get; set; }
    public double Max { get; set; }
    public double Night { get; set; }
    public double Eve { get; set; }
    public double Morn { get; set; }
}

public class FeelsLike
{
    public double Day { get; set; }
    public double Night { get; set; }
    public double Eve { get; set; }
    public double Morn { get; set; }
}
public class WeatherCondition
{
    public int Id { get; set; }
    public string Main { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}
