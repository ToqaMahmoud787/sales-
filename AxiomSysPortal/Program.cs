using AxiomSysPortal.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});

// Razor Pages
builder.Services.AddRazorPages();

// HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

// Backend API Client
builder.Services.AddHttpClient<DeliveryTermsApiClient>(client =>
{
    var apiUrl = builder.Configuration["Api:BaseUrl"]
        ?? throw new InvalidOperationException(
            "Api:BaseUrl is missing.");

    client.BaseAddress = new Uri(apiUrl);
});

// Cookie Authentication
builder.Services
    .AddAuthentication(
        CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/en/Account/Login";
        options.AccessDeniedPath = "/en/Account/AccessDenied";

        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Error Handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Localization for English and Arabic
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ar")
};

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures("en", "ar")
    .AddSupportedUICultures("en", "ar");

localizationOptions.RequestCultureProviders.Insert(
    0,
    new RouteDataRequestCultureProvider()
    {
        RouteDataStringKey = "culture",
        UIRouteDataStringKey = "culture"
    });

app.UseRequestLocalization(localizationOptions);

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Static Files
app.MapStaticAssets();

// Razor Pages
app.MapRazorPages()
   .WithStaticAssets();

app.Run();