using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Devices.Sensors;

namespace WeatherApp;

public class WeatherViewModel : INotifyPropertyChanged
{
    private readonly IOpenWeatherForecastApi _weatherApi;
    private readonly IGeoCodingApi _geoApi;

    public WeatherViewModel(IOpenWeatherForecastApi weatherApi, IGeoCodingApi geoApi)
    {
        _weatherApi = weatherApi;
        _geoApi = geoApi;
        SearchCityCommand = new Command<string>(async (city) => await SearchCityAsync(city));
        RefreshCommand = new Command(async () => await Refresh());
    }

    private bool _isRefreshing;
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set => SetProperty(ref _isRefreshing, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _cityName = string.Empty;
    public string CityName
    {
        get => _cityName;
        set => SetProperty(ref _cityName, value);
    }

    private ObservableCollection<ForecastListItem> _forecast = new();
    public ObservableCollection<ForecastListItem> Forecast
    {
        get => _forecast;
        set => SetProperty(ref _forecast, value);
    }

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool NoForecast => !Forecast.Any() && !IsLoading;
    public bool HasForecast => Forecast.Any();

    public ICommand LoadForecastCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SearchCityCommand { get; }


    public event EventHandler<LocationErrorEventArgs>? onLocationError;

    private double userLat;
    private double userLon;

    private async Task Refresh()
    {
        if (IsRefreshing)
            return;

        try
        {
            IsRefreshing = true;

            await LoadForecastAsync(userLat, userLon);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async Task LoadForecastAsync(double lat, double lon)
    {
        if (IsLoading)
            return;

        try
        {
            UpdateIsLoading(true);

            ErrorMessage = string.Empty;

            Forecast.Clear();

            var response = await _weatherApi.GetForecastAsync(
                lat: 44.34,
                lon: 10.99,
                appid: "df74e6d40a2d3d15b9eb73215f18b63c",
                units: "metric"
            );

            CityName = response.City.Name;
            UpdateForecasts(response.List);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load weather data.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            UpdateIsLoading(false);
        }
    }

    public async Task GetUserLocationAsync()
    {
        try
        {
            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Default,
                Timeout = TimeSpan.FromSeconds(10)
            });

            if (location != null)
            {
                userLat = location.Latitude;
                userLon = location.Longitude;

                await LoadForecastAsync(userLat, userLon);
            }
        }
        catch (FeatureNotSupportedException)
        {
            onLocationError?.Invoke(this, new LocationErrorEventArgs("Geolocation is not supported on this device"));
        }
        catch (PermissionException)
        {
            onLocationError?.Invoke(this, new LocationErrorEventArgs("Location permission denied."));
        }
        catch (Exception)
        {
            onLocationError?.Invoke(this, new LocationErrorEventArgs("Unable to get location."));
        }
    }

    private async Task SearchCityAsync(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return;

        if (IsLoading)
            return;

        try
        {
            UpdateIsLoading(true);

            ErrorMessage = string.Empty;
            Forecast.Clear();

            var cities = await _geoApi.GetCityCoordinatesAsync(cityName, limit: 5, apiKey: "df74e6d40a2d3d15b9eb73215f18b63c");

            var greekCity = cities.FirstOrDefault(c => c.Country == "GR");

            if (greekCity == null)
            {
                ErrorMessage = "No Greek city found with that name.";
                return;
            }

            CityName = greekCity.Name;

            var response = await _weatherApi.GetForecastAsync(
                lat: greekCity.Lat,
                lon: greekCity.Lon,
                appid: "df74e6d40a2d3d15b9eb73215f18b63c",
                units: "metric"
            );

            UpdateForecasts(response.List);
        }
        catch (Exception ex)
        {
            ErrorMessage = "Failed to load weather data.";
            System.Diagnostics.Debug.WriteLine(ex);
        }
        finally
        {
            UpdateIsLoading(false);
        }
    }


    #region Helpers
    private void UpdateIsLoading(bool value) => IsLoading = value;
    public void UpdateForecasts(List<ForecastListItem> newData)
    {
        Forecast.Clear();

        if (newData != null)
        {
            foreach (var f in newData)
                Forecast.Add(f);
        }

        OnPropertyChanged(nameof(NoForecast));
        OnPropertyChanged(nameof(HasForecast));
    }
    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected bool SetProperty<T>(
        ref T backingStore,
        T value,
        [CallerMemberName] string propertyName = "")
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

        backingStore = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void OnPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}

public class LocationErrorEventArgs : EventArgs 
{
    public string ErrorMessage { get; private set; }
    public LocationErrorEventArgs(string errorMessage)
    { 
        this.ErrorMessage = errorMessage;
    }
}