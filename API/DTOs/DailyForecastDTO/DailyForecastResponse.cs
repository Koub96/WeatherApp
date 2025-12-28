using System.Globalization;

public class ForecastResponse
{
    public CityInfo City { get; set; }
    public List<ForecastListItem> List { get; set; }
}

public class CityInfo
{
    public string Name { get; set; }
    public Coord Coord { get; set; }
    public string Country { get; set; }
}

public class Coord
{
    public double Lat { get; set; }
    public double Lon { get; set; }
}


public class ForecastListItem
{
    public long Dt { get; set; }
    public MainInfo Main { get; set; }
    public List<WeatherInfo> Weather { get; set; }
    public CloudsInfo Clouds { get; set; }
    public WindInfo Wind { get; set; }
    public string Dt_txt { get; set; }
    public string LocalDateTime
    {
        get
        {
            if (DateTime.TryParse(Dt_txt, out var utcDateTime))
            {
                var localDateTime = utcDateTime.ToLocalTime(); 
                return localDateTime.ToString("ddd, HH:mm", CultureInfo.CurrentCulture);
            }

            return Dt_txt;
        }
    }
}

public class CloudsInfo
{
    public int All { get; set; } // matches {"all":75}
}

public class MainInfo
{
    public double Temp { get; set; }
    public double Feels_like { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
}

public class WeatherInfo
{
    public int Id { get; set; }
    public string Main { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}

public class WindInfo
{
    public double Speed { get; set; }
    public int Deg { get; set; }
}
