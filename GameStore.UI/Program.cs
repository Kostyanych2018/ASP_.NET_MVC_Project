using System.Security.Claims;
using GameStore.Application.Common;
using GameStore.Application.Common.Constants;
using GameStore.Infrastructure.Authentication;
using GameStore.UI.Constants;
using GameStore.UI.Models.Cart;
using GameStore.UI.Services.Authentication;
using GameStore.UI.Services.Cart;
using GameStore.UI.Services.Games;
using GameStore.UI.Services.Genres;
using GameStore.UI.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection("Keycloak"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.Configure<UiSettings>(builder.Configuration.GetSection("UiSettings"));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddHttpClient<ITokenAccessor, KeycloakTokenAccessor>();
builder.Services.AddTransient<AuthTokenHandler>();

var apiSettings = builder.Configuration.GetSection("ApiSettings").Get<ApiSettings>()!;
var apiBaseUrl = apiSettings.BaseUrl;

builder.Services.AddHttpClient<IGenreService, ApiGenreService>(client =>
    {
        client.BaseAddress = new Uri($"{apiBaseUrl}/api/genres/");

    })
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<IGameService, ApiGameService>(client =>
    {
        client.BaseAddress = new Uri($"{apiBaseUrl}/api/games/");

    })
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<IAuthService, KeycloakAuthService>();

var keycloakSettings = builder.Configuration.GetSection("Keycloak").Get<KeycloakSettings>()!;

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = KeycloakConstants.OpenIdConnectScheme;
    })
    .AddCookie()
    .AddOpenIdConnect(KeycloakConstants.OpenIdConnectScheme, options =>
    {
        options.Authority = $"{keycloakSettings.Host}/realms/{keycloakSettings.Realm}";
        options.ClientId = keycloakSettings.ClientId;
        options.ClientSecret = keycloakSettings.ClientSecret;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.RequireHttpsMetadata = false;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal?.Identity as ClaimsIdentity;
                claimsIdentity.AddKeycloakRoles();
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthConstants.AdminPolicy, policy =>
        policy.RequireRole(AuthConstants.AdminRole));

    options.AddPolicy(AuthConstants.UserPolicy, policy =>
        policy.RequireRole(AuthConstants.UserRole));
});

var app = builder.Build();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();