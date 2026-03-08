using System.Security.Claims;
using PixsyAPI.ErrorHandling;

namespace PixsyAPI.Services.Security;

public static class UserContext
{
    public static int GetUserIdOrThrow(this ClaimsPrincipal user)
    {
        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(idStr) || !int.TryParse(idStr, out var id))
            throw new ForbiddenException("Липсва валиден user id.");
        return id;
    }

    public static int? GetUserIdOrNull(this ClaimsPrincipal user)
    {
        var idStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : null;
    }
}
