using Vossence.ADMIN.Controllers;
using Vossence.DATA.Contexts;
using Vossence.DATA.Identity;
using Vossence.DATA.ORM;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var provider = builder.Services.BuildServiceProvider();
var configuration = provider.GetRequiredService<IConfiguration>();

builder.Services.AddDbContext<MainConnection>(item => item.UseSqlServer(configuration.GetConnectionString("MainConnection"), y => y.MigrationsAssembly("Vossence.ADMIN")));
builder.Services.AddScoped<IDapper, DapperProcess>();
builder.Services.AddIdentity<AppUser, AppRole>(x =>
{
    x.Password.RequiredLength = 6;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireLowercase = false;
    x.Password.RequireUppercase = false;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;

}).AddEntityFrameworkStores<MainConnection>().AddDefaultTokenProviders();
builder.Services.Configure<FormOptions>(x => { x.ValueCountLimit = int.MaxValue; });
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();
builder.Services.AddMvc();
builder.Services.AddTransient<HelperController>();
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(300); });

CookieBuilder cookieBuilder = new CookieBuilder()
{
    Name = "MyCookie",
    HttpOnly = false,
    SameSite = SameSiteMode.Lax,
    SecurePolicy = CookieSecurePolicy.SameAsRequest
};

builder.Services.ConfigureApplicationCookie(x =>
{
    x.LoginPath = new PathString("/app-login");
    x.Cookie = cookieBuilder;
    x.SlidingExpiration = true;
    x.ExpireTimeSpan = System.TimeSpan.FromDays(60);
    x.AccessDeniedPath = new PathString("/access-denied");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseSession();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();