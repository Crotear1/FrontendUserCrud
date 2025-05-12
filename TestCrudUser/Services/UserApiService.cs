using System.Net.Http;
using System.Net.Http.Json; // Für GetFromJsonAsync etc.
using System.Threading.Tasks;
using System.Collections.Generic;
using TestCrudUser.Models;

namespace TestCrudUser.Services
{
    public class UserApiService
    {
        private readonly HttpClient _httpClient;
        // Basis-URL deiner API. Passe den Port an!
        // Für Android Emulator: "http://10.0.2.2:<PORT>" oder "https://10.0.2.2:<PORT>"
        // Für Windows mit WSL/Android Subsystem: IP-Adresse deines PCs im Netzwerk
        // Für iOS Simulator (auf Mac, API läuft auch auf Mac): "https://localhost:<PORT>"
        // Für physische Geräte: IP-Adresse deines PCs im lokalen Netzwerk.
        private string _baseApiUrl = GetApiBaseUrl();

        public UserApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static string GetApiBaseUrl()
        {
            // Passe den Port an, den deine API verwendet (siehe Start des API-Projekts)
            // Beispiel: https://localhost:7123
            // Für lokale Entwicklung mit Emulatoren/Geräten ist es oft besser, die IP-Adresse des PCs zu verwenden
            // anstelle von 'localhost', besonders wenn HTTPS mit Entwicklerzertifikaten genutzt wird.

            // Für Android Emulator, der auf dem gleichen PC wie die API läuft:
            // return DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:7123" : "https://localhost:7123";
            // WICHTIG: Ersetze 7123 mit dem tatsächlichen Port deiner API.
            // Wenn du HTTPS verwendest, stelle sicher, dass das Zertifikat auf dem Gerät/Emulator vertrauenswürdig ist,
            // oder konfiguriere HttpClientHandler, um Zertifikatsfehler für die lokale Entwicklung zu ignorieren (nicht empfohlen für Produktion).

            // Für eine robustere Lösung, insbesondere mit HTTPS und Entwicklerzertifikaten:
            // 1. Stelle sicher, dass deine API über HTTPS mit einem Entwicklerzertifikat läuft (`dotnet dev-certs https --trust`).
            // 2. Verwende die IP-Adresse deines Entwicklungsrechners im lokalen Netzwerk.
            //    Beispiel: "https://192.168.1.100:7123"
            //    Diese IP musst du ggf. in der `applicationUrl` deiner API in `launchSettings.json` hinzufügen:
            //    "applicationUrl": "https://localhost:7123;http://localhost:5123;https://192.168.1.100:7123" (Beispiel-IP)

            // Für den Anfang und Testen mit HTTP (wenn HTTPS Probleme macht und du es für die API temporär deaktivierst):
            // return DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5123" : "http://localhost:5123";
            // Denke daran, die API auch für HTTP zu konfigurieren (siehe launchSettings.json der API).

            // **Empfehlung für Start:** Versuche es mit der HTTPS-URL deiner API (z.B. https://localhost:7123)
            // und passe sie bei Bedarf für Android (10.0.2.2) oder die IP deines PCs an.
            return "https://localhost:7198"; // Passe dies an!
        }


        public async Task<List<User>?> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseApiUrl}/api/users");
                response.EnsureSuccessStatusCode(); // Löst eine Ausnahme aus, wenn der HTTP-Statuscode keinen Erfolg anzeigt
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            catch (HttpRequestException ex)
            {
                // Logge den Fehler oder zeige eine Meldung an
                Console.WriteLine($"Error fetching users: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                // Für Android/iOS kann es sein, dass Cleartext-HTTP blockiert wird oder SSL-Zertifikate nicht vertrauenswürdig sind.
                // Stelle sicher, dass deine API über HTTPS mit einem gültigen (Entwickler-)Zertifikat läuft.
                // Oder konfiguriere die Plattform für Cleartext (siehe unten, falls HTTPS nicht sofort klappt).
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
