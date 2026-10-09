using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace AxiomSysPortal.Pages.Account;

[Authorize]
public class LogoutModel : PageModel
{
    public IActionResult OnGet(string culture) => RedirectToPage("/SALES/DELIVERYTERMS/Index", new { culture });
    public async Task<IActionResult> OnPostAsync(string culture)
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Account/Login", new { culture });
    }
}
