using WEB_353501_Gruganov.UI;
using WEB_353501_Gruganov.UI.Extensions;
using WEB_353501_Gruganov.UI.Services.GameService;

var builder = WebApplication.CreateBuilder(args);
var uriData = builder.Configuration.GetSection("UriData").Get<UriData>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.RegisterCustomServices();

builder.Services.AddHttpClient<IGameService,ApiGameService>(opt=>opt.BaseAddress=new Uri(uriData.ApiUri+"games"));
builder.Services.AddHttpClient<IGenreService,ApiGenreService>(opt=>opt.BaseAddress=new Uri(uriData.ApiUri+"genres"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();