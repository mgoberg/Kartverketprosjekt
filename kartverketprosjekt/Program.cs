using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using kartverketprosjekt.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using kartverketprosjekt.Services;
using kartverketprosjekt.API_Models;
using Discord; // Add this
using Discord.WebSocket;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Identity; // Add this


var builder = WebApplication.CreateBuilder(args);

// Bind the API settings from appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register services and their interfaces
builder.Services.AddHttpClient<IKommuneInfoService, KommuneInfoService>();
builder.Services.AddHttpClient<IStedsnavnService, StedsnavnService>(); // Can be removed if not needed


builder.Services.AddScoped<IAutentiseringService, AutentiseringService>();

builder.Services.AddScoped<PasswordHasher<BrukerModel>>();

builder.Services.AddScoped<IBrukerService, BrukerService>();

builder.Services.AddScoped<INotifikasjonService, NotifikasjonService>();

builder.Services.AddScoped<ISakService, SakService>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IPasswordHasher<BrukerModel>, PasswordHasher<BrukerModel>>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure your DbContext here
builder.Services.AddDbContext<KartverketDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(11, 3, 2)))); // Adjust version as needed

// Enable authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Index";  // Set the login page
        options.LogoutPath = "/Home/Logout"; // Set the logout page
    });

// Add Discord services
builder.Services.AddScoped<DiscordBot>(); // Register DiscordBot as scoped
builder.Services.AddSingleton(new DiscordSocketClient()); // Register DiscordSocketClient as singleton

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

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
    var discordBot = scope.ServiceProvider.GetRequiredService<DiscordBot>();
    var discordToken = builder.Configuration["Discord:Token"]; // Get the Discord token from configuration
    await discordBot.StartAsync(discordToken); // Pass the token when starting the bot
}

app.Run();
