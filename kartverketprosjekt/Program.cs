using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using kartverketprosjekt.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using kartverketprosjekt.API_Models;
using Discord; // Add this
using Discord.WebSocket;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Identity;
using kartverketprosjekt.Services.Admin;
using kartverketprosjekt.Services.API;
using kartverketprosjekt.Services.Sak;
using kartverketprosjekt.Services.Autentisering;
using kartverketprosjekt.Services.Bruker;
using kartverketprosjekt.Services.File;
using kartverketprosjekt.Services.Kommentar;
using kartverketprosjekt.Services.Notifikasjon;
using kartverketprosjekt.Repositories.Bruker;
using kartverketprosjekt.Repositories.Kommentar;
using kartverketprosjekt.Repositories.Notifikasjon;
using kartverketprosjekt.Repositories.Sak; // Add this


var builder = WebApplication.CreateBuilder(args);

// Bind the API settings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddHttpClient<IKommuneInfoService, KommuneInfoService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
// Services og tilsvarende interface
builder.Services.AddScoped<IAutentiseringService, AutentiseringService>();
builder.Services.AddScoped<PasswordHasher<BrukerModel>>();
builder.Services.AddScoped<IBrukerService, BrukerService>();
builder.Services.AddScoped<INotifikasjonService, NotifikasjonService>();
builder.Services.AddScoped<ISakService, SakService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IKommentarService, KommentarService>();
builder.Services.AddScoped<IPasswordHasher<BrukerModel>, PasswordHasher<BrukerModel>>();

// Repositories og tilsvarende interface
builder.Services.AddScoped<IBrukerRepository, BrukerRepository>();
builder.Services.AddScoped<ISakRepository, SakRepository>();
builder.Services.AddScoped<IKommentarRepository, KommentarRepository>();
builder.Services.AddScoped<INotifikasjonRepository, NotifikasjonRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database konfigurasjon
builder.Services.AddDbContext<KartverketDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(11, 3, 2)))); 

// Enable authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";  // Set the login page
        options.LogoutPath = "/Home/Logout"; // Set the logout page
    });

// Add Discord services
builder.Services.AddScoped<IDiscordBot, DiscordBot>(); // Register DiscordBot as scoped
builder.Services.AddSingleton(new DiscordSocketClient()); // Register DiscordSocketClient as singleton

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseCsp(options => options
    .DefaultSources(s => s.Self())
    .ScriptSources(s => s.Self()
        .CustomSources("https://unpkg.com/leaflet@1.9.4/dist/leaflet.js",
            "https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js",
            "https://cdn-geoweb.s3.amazonaws.com/esri-leaflet/0.0.1-beta.5/esri-leaflet.js",
            "https://cdn-geoweb.s3.amazonaws.com/esri-leaflet-geocoder/0.0.1-beta.5/esri-leaflet-geocoder.js",
            "https://cdnjs.cloudflare.com/ajax/libs/leaflet.draw/0.4.2/leaflet.draw.js",
            "https://code.jquery.com/jquery-3.6.0.min.js",
            "https://unpkg.com/leaflet@1.7.1/dist/leaflet.js")
        .UnsafeInline())
    .StyleSources(s => s.Self()
        .CustomSources("https://unpkg.com/boxicons@2.1.4/css/boxicons.min.css",
            "https://unpkg.com/leaflet@1.9.4/dist/leaflet.css",
            "https://unpkg.com/leaflet@1.7.1/dist/leaflet.css",
            "https://cdn-geoweb.s3.amazonaws.com/esri-leaflet-geocoder/0.0.1-beta.5/esri-leaflet-geocoder.css",
            "https://cdnjs.cloudflare.com/ajax/libs/leaflet.draw/0.4.2/leaflet.draw.css",
            "https://fonts.googleapis.com",
            "https://fonts.gstatic.com")
        .UnsafeInline())
    .FontSources(s => s.Self()
        .CustomSources("https://fonts.googleapis.com", "https://fonts.gstatic.com", "https://unpkg.com"))
    .ImageSources(s => s.Self()
        .CustomSources("data:", "https://cache.kartverket.no",
            "https://cdn-geoweb.s3.amazonaws.com/esri-leaflet-geocoder/0.0.1-beta.5/img/search.png",
            "https://cdnjs.cloudflare.com/ajax/libs/leaflet.draw/0.4.2/images/spritesheet.svg",
            "https://unpkg.com",
            "https://cdn-geoweb.s3.amazonaws.com/esri-leaflet-geocoder/0.0.1-beta.5/img/loading.gif",
            "https://openwms.statkart.no",
            "https://wms.geonorge.no"))
    .ConnectSources(s => s.Self()
        .CustomSources("https://github.com/mgoberg/kartverketprosjekt",
            "https://geocode.arcgis.com"))
    .FrameSources(s => s.Self())
);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Enable authentication middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize the Discord bot within a scope
using (var scope = app.Services.CreateScope())
{
    var discordBot = scope.ServiceProvider.GetRequiredService<IDiscordBot>();
    var discordToken = builder.Configuration["Discord:Token"]; // Get the Discord token from configuration
    await discordBot.StartAsync(discordToken); // Pass the token when starting the bot
}

app.Run();
