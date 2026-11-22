using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalWebSite.Business.Interfaces;
using PersonalWebSite.Business.Services;
using PersonalWebSite.Core.Models;
using PersonalWebSite.Data;
using PersonalWebSite.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<PersonalWebSiteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<PersonalWebSiteDbContext>()
.AddDefaultTokenProviders();

// Add Services
builder.Services.AddScoped<IQuestionService, QuestionService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();

// Configure cookie policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = false;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseMiddleware<RequestLoggingMiddleware>();

// 404 gibi boş yanıtları şık hata sayfasına yönlendir
app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Roller ve (yapılandırmada tanımlıysa) admin kullanıcısı
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<User>>();

        // Kayıt olan kullanıcılar "User" rolüne eklenir, bu yüzden iki rol de var olmalı.
        foreach (var role in new[] { "Admin", "User" })
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Admin bilgileri koda gömülü değil; AdminUser:* ayarlarından okunur
        // (appsettings.Development.json, user-secrets veya ortam değişkenleri).
        var adminEmail = builder.Configuration["AdminUser:Email"];
        var adminPassword = builder.Configuration["AdminUser:Password"];

        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword))
        {
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = builder.Configuration["AdminUser:FirstName"] ?? "Admin",
                    LastName = builder.Configuration["AdminUser:LastName"] ?? "User"
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (!result.Succeeded)
                {
                    logger.LogError("Admin kullanıcısı oluşturulamadı: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    adminUser = null;
                }
            }

            if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
        else
        {
            logger.LogWarning("AdminUser:Email / AdminUser:Password ayarlanmadığı için admin kullanıcısı oluşturulmadı.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Roller veya admin kullanıcısı oluşturulurken bir hata oluştu.");
    }
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
