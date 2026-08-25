using GameStore.API.Common;
using GameStore.API.Extensions;
using GameStore.API.Middlewares;
using GameStore.Application;
using GameStore.Application.Common.Interfaces;
using GameStore.Infrastructure;
using GameStore.Infrastructure.Data;
using GameStore.Infrastructure.Settings;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(builder.Configuration);

builder.Services.AddScoped<IUserContext, CurrentUserContext>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ExceptionHandlingMiddleware>();
builder.Services.AddControllers();

builder.Services.Configure<FileStorageSettings>(options => { options.BasePath = builder.Environment.WebRootPath; });

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo()
        {
            Title = "GameStore API",
            Version = "v1",
            Description = "GameStore ASP .NET Web API with Keycloak"
        };

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter JWT Bearer token. Example: Bearer eyJhbGciOi...",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };
        document.Components.SecuritySchemes.Add("Bearer", securityScheme);
        return Task.CompletedTask;
    });
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitializeAsync();
}

app.UseExceptionHandler();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("GameStore API Documentation");
        options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();