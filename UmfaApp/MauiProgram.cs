using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using System.Reflection;
using UmfaApp.Data;
using UmfaApp.Services;

namespace UmfaApp
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
                });

            // appsettings
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("UmfaApp.appsettings.json");

            var config = new ConfigurationBuilder()
                        .AddJsonStream(stream)
                        .Build();


            builder.Configuration.AddConfiguration(config);

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<DbAccessor>();
            builder.Services.AddSingleton<IUmfaService, UmfaApiHttpService>();
            builder.Services.AddSingleton<ILogInService, LogInService>();
            builder.Services.AddSingleton<IReadingListService, ReadingListService>();
            builder.Services.AddSingleton<IPartnerService, PartnerService>();
            builder.Services.AddSingleton<IActionLogService, ActionLogService>();


            //record and play audio
            builder.Services.AddSingleton(AudioManager.Current);

            builder.Services.AddBlazorBootstrap();

            Helpers.ServiceLocator.ServiceProvider = builder.Services.BuildServiceProvider();

            return builder.Build();
        }
    }
}