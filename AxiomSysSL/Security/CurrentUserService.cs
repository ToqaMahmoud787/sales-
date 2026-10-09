using System.Security.Claims;

namespace AxiomSysSL.Security;

public interface ICurrentUserService
{
    long? UserId { get; }
}

public sealed class CurrentUserService(IHttpContextAccessor accessor)
    : ICurrentUserService
{
    public long? UserId =>
        long.TryParse(accessor.HttpContext?.User.FindFirstValue("Id"), out var id)
        && id > 0 && id <= 9_999_999_999L ? id : null;
}
