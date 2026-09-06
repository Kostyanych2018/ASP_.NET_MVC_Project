using GameStore.Application.Common.Constants;
using GameStore.Application.Common.Exceptions;
using GameStore.Application.Common.Interfaces;

namespace GameStore.Application.Common.Extensions;

public static class UserContextExtensions
{
    public static void EnsureAdmin(this IUserContext userContext)
    {
        if (!userContext.IsInRole(AuthConstants.AdminRole))
        {
            throw new ForbiddenException("Access denied. Administrator role is required.");
        }
    }
}
