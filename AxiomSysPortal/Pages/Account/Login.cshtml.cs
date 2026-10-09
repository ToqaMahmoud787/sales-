using System.Security.Claims;
using AxiomSysPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace AxiomSysPortal.Pages.Account;

[AllowAnonymous]
public class LoginModel(DeliveryTermsApiClient api,
    IStringLocalizer<SharedResource> localizer, ILogger<LoginModel> logger) : PageModel
{
    [BindProperty] public string Username { get; set; } = "";
    [BindProperty] public string Password { get; set; } = "";
    public string? Error { get; set; }
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync(string culture)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrEmpty(Password))
        {
            Error = localizer["RequiredFields"]; return Page();
        }
        try
        {
            var result = await api.LoginAsync(Username, Password);
            if (result == null) { Error = localizer["InvalidLogin"]; return Page(); }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, result.Username),
                new Claim("CompanyCode", result.CompanyCode)
            };
            var properties = new AuthenticationProperties
            {
                ExpiresUtc = result.ExpiresUtc, IsPersistent = false, AllowRefresh = false
            };
            properties.StoreTokens([new AuthenticationToken
            {
                Name = "access_token", Value = result.Token
            }]);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme)), properties);
            return RedirectToPage("/SALES/DELIVERYTERMS/Index", new { culture });
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            logger.LogError(ex, "Sign-in API request failed.");
            Error = localizer["ApiUnavailable"];
            return Page();
        }
    }
}
