using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.API.EndPoints;
using WEB_353501_Gruganov.API.Models;


var builder = WebApplication.CreateBuilder(args);
var authServer = builder.Configuration
    .GetSection("AuthServer")
    .Get<AuthServerData>()!;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        // options.MetadataAddress = $"{authServer.Host}/realms/{authServer.Realm}/.wellknown/openid-configuration";
        options.Authority = $"{authServer.Host}/realms/{authServer.Realm}";
        options.Audience = "account";
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin",p=>p.RequireRole("POWER-USER"));
});

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();


await DbInitializer.SeedData(app);

app.MapControllers();

app.UseStaticFiles();

app.MapGenreEndpoints();

app.MapGameEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}


// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();