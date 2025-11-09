using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using WEB_353501_Gruganov.UI;
using WEB_353501_Gruganov.UI.Extensions;
using WEB_353501_Gruganov.UI.HelperClasses;
using WEB_353501_Gruganov.UI.Services.Authentication;
using WEB_353501_Gruganov.UI.Services.GameService;

var builder = WebApplication.CreateBuilder(args);
var uriData = builder.Configuration
    .GetSection("UriData")
    .Get<UriData>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.RegisterCustomServices();
builder.Services.AddRazorPages();

var keycloakData = builder.Configuration
    .GetSection("Keycloak")
    .Get<KeycloakData>()!;

builder.Services.AddHttpClient<IGameService,ApiGameService>(opt=>opt.BaseAddress=new Uri(uriData.ApiUri+"games"));
builder.Services.AddHttpClient<IGenreService,ApiGenreService>(opt=>opt.BaseAddress=new Uri(uriData.ApiUri+"genres"));
builder.Services.AddHttpClient<ITokenAccessor, KeycloakTokenAccessor>();

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = "keycloak";
    })
    .AddCookie()
    .AddOpenIdConnect("keycloak", options =>
    {
        options.Authority = $"{keycloakData.Host}/auth/realms/{keycloakData.Realm}";
        options.ClientId = keycloakData.ClientId;
        options.ClientSecret = keycloakData.ClientSecret;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.Scope.Add("openid"); 
        options.SaveTokens = true;
        options.RequireHttpsMetadata = false; 
        options.MetadataAddress =
            $"{keycloakData.Host}/realms/{keycloakData.Realm}/.well-known/openid-configuration";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin",p=>p.RequireRole("POWER-USER"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .RequireAuthorization("admin");

app.Run();