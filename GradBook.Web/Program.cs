using GradBook.Application.Interfaces;
using GradBook.Infrastructure.Data;
using GradBook.Infrastructure.Services;
using GradBook.Web.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ── DATABASE ──
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl))
{
    // تغيير الاسم إلى connBuilder لتجنب التعارض مع builder الخاص بالتطبيق
    var connBuilder = new NpgsqlConnectionStringBuilder(databaseUrl);

    connBuilder.SslMode = SslMode.Require;
    connBuilder.TrustServerCertificate = true;

    connectionString = connBuilder.ToString();
}
else
{
    // هنا نستخدم builder الخاص بالتطبيق (الموجود مسبقاً)
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