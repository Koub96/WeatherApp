using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel;

namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        #region Constructor
        public MainPage()
        {
            InitializeComponent();

            WeatherViewModel? viewModel = Application.Current?.Handler.MauiContext?.Services.GetRequiredService<WeatherViewModel>();

            BindingContext = viewModel;
        }
        #endregion

        #region Overrides
        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is not null && BindingContext is WeatherViewModel)
            {
                (BindingContext as WeatherViewModel).onLocationError += OnLocationError;
                (BindingContext as WeatherViewModel).onSearchError += OnSearchError;
                (BindingContext as WeatherViewModel).onForecastsLoadError += OnForecastsLoadError;
                await (BindingContext as WeatherViewModel).GetUserLocationAsync();
            }
        }
        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);

            if (BindingContext is not null && BindingContext is WeatherViewModel)
            {
                (BindingContext as WeatherViewModel).onLocationError -= OnLocationError;
                (BindingContext as WeatherViewModel).onSearchError -= OnSearchError;
                (BindingContext as WeatherViewModel).onForecastsLoadError -= OnForecastsLoadError;
            }
        }
        #endregion

        #region Events
        private async void OnLocationError(object? sender, LocationErrorEventArgs e)
        {
            await ShowSnackbar(e.ErrorMessage, actionButtonText: "Settings", action: () => AppInfo.ShowSettingsUI());
        }
        private async void OnSearchError(object? sender, SearchErrorEventArgs e)
        {
            await ShowSnackbar(e.ErrorMessage);
        }
        private async void OnForecastsLoadError(object? sender, LoadForecastsErrorEventArgs e)
        {
            await ShowSnackbar(e.ErrorMessage);
        }
        #endregion

        #region Helpers
        private async Task ShowSnackbar(string message, string actionButtonText = "", Action? action = null)
        {
            var snackbar = Snackbar.Make(
                actionButtonText: actionButtonText,
                action: action,
                message: message,
                duration: TimeSpan.FromSeconds(3.5)
            );

            await snackbar.Show();
        }
        #endregion
    }
}
