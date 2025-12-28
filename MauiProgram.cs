using Microsoft.Extensions.Logging;

namespace WeatherApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services
            .AddRefitClient<IOpenWeatherForecastApi>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5");
            });
            return builder.Build();
        }
    }
}
