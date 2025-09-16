using System.Text;
using System.Text.Json;

namespace Texta.Utils
{
    public class LanguageDetectionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public LanguageDetectionService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string?> DetectLanguageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var requestBody = new { q = text };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            var response = await _httpClient.PostAsync("https://ws.detectlanguage.com/0.2/detect", content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API error: {response.StatusCode} - {error}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseContent);
            return doc.RootElement
                      .GetProperty("data")
                      .GetProperty("detections")[0]
                      .GetProperty("language")
                      .GetString();
        }            
    }
}