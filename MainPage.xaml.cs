using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui.ApplicationModel;

namespace WeatherApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            WeatherViewModel? viewModel = Application.Current?.Handler.MauiContext?.Services.GetRequiredService<WeatherViewModel>();

            BindingContext = viewModel;
        }

        protected async override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            if (BindingContext is not null && BindingContext is WeatherViewModel)
            {
                (BindingContext as WeatherViewModel).onLocationError += OnLocationError;
                await (BindingContext as WeatherViewModel).GetUserLocationAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);

            if (BindingContext is not null && BindingContext is WeatherViewModel)
            {
                (BindingContext as WeatherViewModel).onLocationError -= OnLocationError;
            }
        }

        private async void OnLocationError(object? sender, LocationErrorEventArgs e)
        {
            var snackbar = Snackbar.Make(
                actionButtonText: "Settings",
                action: () => AppInfo.ShowSettingsUI(),
                message: e.ErrorMessage,
                duration: TimeSpan.FromSeconds(3.5)
            );

            await snackbar.Show();
        }
    }
}
