using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using AgriEnergy_ConnectApp.Data;
using AgriEnergy_ConnectApp.Models;
using Microsoft.EntityFrameworkCore.Proxies;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(); // Add session support

// Register ApplicationDbContext (Database Context)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register IHttpContextAccessor (useful for accessing HttpContext in controllers)
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add authentication services (Cookie-based authentication)
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login"; // Redirect to the login page
        options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect to access denied page
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expiration time
        options.SlidingExpiration = true; // Renew cookie expiration on activity
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) // Check for Development environment
{
    app.UseDeveloperExceptionPage(); // Show detailed error page in Development mode
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Use generic error handler in production
    app.UseHsts(); // Enforce HTTPS
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS
app.UseStaticFiles(); // Enable static files like CSS, JavaScript, images

app.UseRouting(); // Enable routing

// Enable session and authentication middleware
app.UseSession();
app.UseAuthentication(); // Add authentication middleware
app.UseAuthorization(); // Add authorization middleware

// Define default route mapping
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
