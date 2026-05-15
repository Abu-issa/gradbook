using GradBook.Application.Interfaces;
using GradBook.Infrastructure.Data;
using GradBook.Infrastructure.Services;
using GradBook.Web.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Npgsql;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ── DATABASE ──
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // تحويل PostgreSQL URI إلى connection string
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');

    connectionString = new NpgsqlConnectionStringBuilder
    {
        Host = uri.Host,
        Port = uri.Port,
        Database = uri.AbsolutePath.TrimStart('/'),
        Username = userInfo[0],
        Password = userInfo[1],
        SslMode = SslMode.Require,
        TrustServerCertificate = true
    }.ToString();
}
else
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
}
builder.Services.AddDbContext<GradBookDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IMemoryService, MemoryService>();
builder.Services.AddScoped<IVisitorService, VisitorService>();

// Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.AccessDeniedPath = "/Admin/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddSignalR();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MessageHub>("/messageHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ── CREATE DB ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GradBookDbContext>();
    db.Database.EnsureCreated();
}

app.Run();