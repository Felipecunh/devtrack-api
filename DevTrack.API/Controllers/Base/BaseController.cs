using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevTrack.API.Controllers.Base;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected Guid UserId
    {
        get
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            return Guid.Parse(userId);
        }
    }
}
