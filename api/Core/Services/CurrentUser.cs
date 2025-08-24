using System.Security.Claims;
using Waffle.Core.Interfaces;

namespace Waffle.Core.Services;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _contextAccessor;
    public CurrentUser(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Guid GetId()
    {
        var userId = _contextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return Guid.Empty;
        return Guid.Parse(userId);
    }

    public bool IsInRole(string roleName) => _contextAccessor.HttpContext?.User.IsInRole(roleName) ?? false;
}
