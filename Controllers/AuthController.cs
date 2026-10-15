using Komuta.Api.Data;
using Komuta.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Komuta.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(KomutaDbContext db) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await db.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == request.Username && x.Password == request.Password);
        return user is null
            ? Unauthorized()
            : Ok(new CurrentUserDto(user.Username, user.Role));
    }
}
