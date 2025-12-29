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

    #region Bindable Properties
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

    public bool NoForecast => !Forecast.Any() && !IsLoading;
    public bool HasForecast => Forecast.Any();

    public ICommand LoadForecastCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SearchCityCommand { get; }

    public event EventHandler<LocationErrorEventArgs>? onLocationError;
    public event EventHandler<SearchErrorEventArgs>? onSearchError;
    public event EventHandler<LoadForecastsErrorEventArgs>? onForecastsLoadError;
    #endregion

    #region Private Properties
    private double _userLat;
    private double _userLon;

    //This property should be stored server side as it can be decompiled and stolen.
    private const string _apiKey = "df74e6d40a2d3d15b9eb73215f18b63c";
    #endregion

    #region Commands
    private async Task Refresh()
    {
        if (IsRefreshing)
            return;

        try
        {
            IsRefreshing = true;

            await LoadForecastAsync(_userLat, _userLon);
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

            Forecast.Clear();

            var response = await _weatherApi.GetForecastAsync(
                lat: lat,
                lon: lon,
                appid: _apiKey,
                units: "metric"
            );

            CityName = response.City.Name;
            UpdateForecasts(response.List);
        }
        catch (Exception ex)
        {
            onForecastsLoadError?.Invoke(this, new LoadForecastsErrorEventArgs("Could not load forecasts. Please try again."));
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
                _userLat = location.Latitude;
                _userLon = location.Longitude;

                await LoadForecastAsync(_userLat, _userLon);
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

            var cities = await _geoApi.GetCityCoordinatesAsync(cityName, limit: 5, apiKey: _apiKey);

            var greekCity = cities.FirstOrDefault(c => c.Country == "GR");

            if (greekCity == null)
            {
                onSearchError?.Invoke(this, new SearchErrorEventArgs("City not found."));
                return;
            }

            CityName = greekCity.Name;

            var response = await _weatherApi.GetForecastAsync(
                lat: greekCity.Lat,
                lon: greekCity.Lon,
                appid: _apiKey,
                units: "metric"
            );

            UpdateForecasts(response.List);
        }
        catch (Exception ex)
        {
            onSearchError?.Invoke(this, new SearchErrorEventArgs("Search failed. Please try again."));
        }
        finally
        {
            UpdateIsLoading(false);
        }
    }
    #endregion

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

#region Event Args
public class LocationErrorEventArgs : EventArgs 
{
    public string ErrorMessage { get; private set; }
    public LocationErrorEventArgs(string errorMessage)
    { 
        this.ErrorMessage = errorMessage;
    }
}
public class SearchErrorEventArgs : EventArgs
{
    public string ErrorMessage { get; private set; }
    public SearchErrorEventArgs(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }
}
public class LoadForecastsErrorEventArgs : EventArgs
{
    public string ErrorMessage { get; private set; }
    public LoadForecastsErrorEventArgs(string errorMessage)
    {
        this.ErrorMessage = errorMessage;
    }
}
#endregion