using Microsoft.Extensions.Logging;
using WaliSync.Services;

namespace WaliSync
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

            builder.Services.AddMauiBlazorWebView();

            // 1. Toast Notification Service (Singleton across app)
            builder.Services.AddSingleton<ToastService>();

            // 2. Base URL API Configuration
            // Saat demo di HP fisik Android via USB kabel: jalankan 'adb reverse tcp:5000 tcp:5000'
            var baseApiUrl = "http://localhost:5000/";

            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri(baseApiUrl),
                Timeout = TimeSpan.FromSeconds(10)
            });

            // 3. Register Production HTTP Client Services (Connected to WaliSync.Api)
            builder.Services.AddScoped<IDosenService, HttpDosenService>();
            builder.Services.AddScoped<IMahasiswaService, HttpMahasiswaService>();
            builder.Services.AddScoped<IDashboardService, HttpDashboardService>();

            // Optional Stub Services retained for standalone offline testing
            builder.Services.AddSingleton<StubDataStore>();
            builder.Services.AddScoped<StubDosenService>();
            builder.Services.AddScoped<StubMahasiswaService>();
            builder.Services.AddScoped<StubDashboardService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

