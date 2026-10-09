using Microsoft.AspNetCore.Authorization;

namespace AxiomSysSL.Security;

// Standalone substitute, not the original application's implementation.
// Restricts this single-schema demo to the configured company claim.
public sealed class AuthorizeCompanyAttribute : AuthorizeAttribute
{
    public AuthorizeCompanyAttribute() => Policy = "CompanyScope";
}
