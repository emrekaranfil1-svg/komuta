using Komuta.Api.Models;
using Komuta.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Komuta.Api.Controllers;

[ApiController]
[Route("api/turnstile")]
public class TurnstileController(PresenceService presence) : ControllerBase
{
    [HttpPost("swipe")]
    public async Task<IActionResult> Swipe(SwipeRequest request)
    {
        try
        {
            return Ok(await presence.SwipeAsync(request.CardId));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost("random")]
    public async Task<IActionResult> Random() => Ok(await presence.RandomSwipeAsync());
}
