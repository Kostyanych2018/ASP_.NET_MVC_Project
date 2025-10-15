using Microsoft.EntityFrameworkCore;
using WEB_353501_Gruganov.API.Data;
using WEB_353501_Gruganov.API.EndPoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");

builder.Services.AddDbContext<AppDbContext>(options=>options.UseNpgsql(connectionString));

builder.Services.AddMediatR(configuration=>configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));

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


app.UseHttpsRedirection();

app.Run();
