using System.Globalization;
using System.Text.Json;
using AxiomSysPortal.Models;
using AxiomSysPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace AxiomSysPortal.Pages.SALES.DELIVERYTERMS;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public class IndexModel(DeliveryTermsApiClient api,
    IStringLocalizer<SharedResource> localizer, ILogger<IndexModel> logger) : PageModel
{
    public List<DeliveryTerm> Terms { get; set; } = [];
    [BindProperty] public DeliveryTerm Input { get; set; } = new();
    [TempData] public string? SuccessMessage { get; set; }
    [TempData] public string? ErrorMessage { get; set; }
    private string Culture => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var response = await api.ListAsync();
            if (response.Status)
                Terms = response.Data.Deserialize<List<DeliveryTerm>>(DeliveryTermsApiClient.JsonOptions) ?? [];
            else ErrorMessage = Message(response);
            return Page();
        }
        catch (ApiSessionExpiredException) { return await SignInAgain(); }
        catch (Exception ex) when (IsApiError(ex))
        {
            logger.LogError(ex, "Loading delivery terms failed.");
            ErrorMessage = localizer["ApiUnavailable"];
            return Page();
        }
    }

    public async Task<IActionResult> OnGetEditAsync(int code)
    {
        try
        {
            var response = await api.GetAsync(code);
            if (!response.Status)
                return new JsonResult(new { status = false, message = Message(response) }) { StatusCode = 404 };
            return new JsonResult(new { status = true, data = response.Data });
        }
        catch (ApiSessionExpiredException)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new { status = false }) { StatusCode = 401 };
        }
        catch (Exception ex) when (IsApiError(ex))
        {
            logger.LogError(ex, "Loading edit record failed.");
            return new JsonResult(new { status = false, message = localizer["ApiUnavailable"].Value }) { StatusCode = 503 };
        }
    }

    public Task<IActionResult> OnPostAddAsync() => SaveAsync(() => api.AddAsync(Input));
    public Task<IActionResult> OnPostEditAsync() => SaveAsync(() => api.EditAsync(Input));
    public Task<IActionResult> OnPostDeleteAsync(int code) => SaveAsync(() => api.DeleteAsync(code), false);

    private async Task<IActionResult> SaveAsync(Func<Task<ApiResponse>> action, bool validate = true)
    {
        if (validate && !ModelState.IsValid)
        {
            ErrorMessage = localizer["InvalidInput"];
            return RedirectToPage(new { culture = Culture });
        }
        try
        {
            var response = await action();
            if (response.Status) SuccessMessage = Message(response);
            else ErrorMessage = Message(response);
        }
        catch (ApiSessionExpiredException) { return await SignInAgain(); }
        catch (Exception ex) when (IsApiError(ex))
        {
            logger.LogError(ex, "Saving delivery term failed.");
            ErrorMessage = localizer["ApiUnavailable"];
        }
        return RedirectToPage(new { culture = Culture });
    }

    private string Message(ApiResponse response)
    {
        var message = Culture == "ar" ? response.MessageAr : response.MessageEn;
        return string.IsNullOrWhiteSpace(message) ? localizer["ApiUnavailable"].Value : message;
    }
    private async Task<IActionResult> SignInAgain()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Account/Login", new { culture = Culture });
    }
    private static bool IsApiError(Exception ex) =>
        ex is HttpRequestException or TaskCanceledException or JsonException;
}
