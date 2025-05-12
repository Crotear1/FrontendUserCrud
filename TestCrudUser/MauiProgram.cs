// MauiProgram.cs (Auszug)
//using TestCrudUser.Data; // Ggf. für lokale Daten
using TestCrudUser.Services; // Hinzufügen für UserApiService
using TestCrudUser.Models;   // Hinzufügen für User-Modell, falls es dort liegt
using Microsoft.Extensions.Logging;
using TestCrudUser;


public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder ConfigureHttpClient(this MauiAppBuilder builder)
    {
        // HttpClient für UserApiService registrieren
        builder.Services.AddHttpClient<UserApiService>(client =>
        {
            // Die Basis-URL wird jetzt im UserApiService selbst dynamischer ermittelt
            // client.BaseAddress = new Uri("DEINE_API_BASIS_URL"); // Nicht mehr hier, wenn im Service gehandhabt
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
#if DEBUG
            // Erlaube selbstsignierte Zertifikate für localhost im DEBUG-Modus
            // Dies ist nützlich, wenn deine API lokal über HTTPS mit einem Entwicklerzertifikat läuft.
            if (DeviceInfo.Platform == DevicePlatform.Android) // Für Android speziell
            {
                // Für Android kann es erforderlich sein, den Handler spezifischer zu konfigurieren,
                // wenn 10.0.2.2 mit einem "localhost"-Zertifikat verwendet wird.
                // Oft ist es einfacher, die ServerCertificateCustomValidationCallback zu verwenden.
            }
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                // Überprüfe, ob das Zertifikat von "CN=localhost" ausgestellt wurde
                // oder ob keine SSL-Fehler vorliegen.
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };
#endif
            return handler;
        });

        builder.Services.AddSingleton<UserApiService>(); // Den Service selbst registrieren

        return builder;
    }
}


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

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // HttpClient und UserApiService registrieren
        builder.ConfigureHttpClient(); // Aufruf der Erweiterungsmethode

        // Beispiel für einen lokalen Datendienst, falls du einen hast
        // builder.Services.AddSingleton<WeatherForecastService>();

        return builder.Build();
    }
}