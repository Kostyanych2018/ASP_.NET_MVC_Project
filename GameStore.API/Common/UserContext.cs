using System.Security.Claims;
using GameStore.Application.Common.Interfaces;

namespace GameStore.API.Common;

public class UserContext: IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public string? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }

    public string? UserName
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirst(ClaimTypes.Name)?.Value;
        }
    }

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User.IsInRole(role) ?? false;
}