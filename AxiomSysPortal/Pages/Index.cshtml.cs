using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace AxiomSysPortal.Pages;
public class IndexModel : PageModel
{
    public IActionResult OnGet() => RedirectToPage("/SALES/DELIVERYTERMS/Index", new { culture = "en" });
}
