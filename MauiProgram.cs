using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Refit;

namespace WeatherApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddTransient<WeatherViewModel>();
            builder.Services.AddTransient<MainPage>();

            builder.Services
            .AddRefitClient<IOpenWeatherForecastApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5");
            });

            builder.Services.AddRefitClient<IGeoCodingApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://api.openweathermap.org"));

            return builder.Build();
        }
    }
}
