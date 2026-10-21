// ST10445832
// Alex Eduard Pluddemann

// Consolodated Reference List
// https://geeksarray.com/blog/how-to-use-areas-in-asp-net-core-mvc
// https://getbootstrap.com/docs/5.3/components/modal/
// https://getbootstrap.com/docs/5.3/content/tables/
// https://icons.getbootstrap.com/icons/check/
// https://icons.getbootstrap.com/icons/person-circle/
// https://icons.getbootstrap.com/icons/x/
// https://jakeydocs.readthedocs.io/en/latest/fundamentals/error-handling.html
// https://www.learnentityframeworkcore.com/relationships
// https://learn.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-9.0
// https://learn.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=data-annotations
// https://medium.com/@roshanj100/users-and-roles-seeding-in-asp-net-core-identity-with-entity-framework-core-a-step-by-step-guide-28e6f76a18db

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ST10445832_PROG6212_POE.Data;
using Microsoft.AspNetCore.Authorization;
using ST10445832_PROG6212_POE.Models;
using ST10445832_PROG6212_POE.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("LocalDB")));

builder.Services.AddScoped<ILecturerRepository, LecturerRepository>();
builder.Services.AddScoped<IApproverRepository, ApproverRepository>();
builder.Services.AddSignalR();
builder.Services.AddScoped<StatusListener>();
// Custom identity for CMCS
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/SignIn";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// set QuestPDF license to enable document generation
QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();

    await dbContext.Database.MigrateAsync();
    await IdentitySeeder.SeedRolesAndUsers(services);
    await SampleDataSeeder.InitialiseSampleData(services);
}
;

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=SignIn}/{id?}");

app.MapHub<StatusHub>("/StatusHub");

app.Run();
///////////////////////////////////////////////////////////END OF FILE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\