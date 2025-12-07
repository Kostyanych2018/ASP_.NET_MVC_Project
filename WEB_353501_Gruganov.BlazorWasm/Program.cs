using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WEB_353501_Gruganov.BlazorWasm;
using WEB_353501_Gruganov.BlazorWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var apiUrl = builder.Configuration.GetSection("Api:Url").Value;
Console.WriteLine($"API BaseAddress: {apiUrl}");

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });
builder.Services.AddScoped<IDataService, DataService>();
builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("Keycloak", options.ProviderOptions);
    options.UserOptions.NameClaim = "preferred_username";
});

await builder.Build().RunAsync();