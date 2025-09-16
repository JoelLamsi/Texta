using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Texta;
using Texta.Utils;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

using var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

string configFile = File.Exists("wwwroot/appsettings.Development.json")
    ? "appsettings.Development.json"
    : "appsettings.json";

var settings = await httpClient.GetFromJsonAsync<AppSettings>(configFile);

builder.Services.AddScoped<LanguageDetectionService>(sp =>
{
    var client = sp.GetRequiredService<HttpClient>();
    return new LanguageDetectionService(client, settings!.DetectLanguage.ApiKey);
});
await builder.Build().RunAsync();

public class AppSettings
{
    public DetectLanguageSettings DetectLanguage { get; set; } = new();
}

public class DetectLanguageSettings
{
    public string ApiKey { get; set; } = string.Empty;
}