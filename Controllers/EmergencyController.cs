using Komuta.Api.Models;
using Komuta.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Komuta.Api.Controllers;

[ApiController]
[Route("api/emergency")]
public class EmergencyController(EmergencyService emergency) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Set(ModeRequest request) =>
        Ok(new { mode = await emergency.SetAsync(request.Mode), emergencyActive = request.Mode != "normal" });

    [HttpPost("activate")]
    public async Task<IActionResult> Activate() =>
        Ok(new { mode = await emergency.SetAsync("emergency"), emergencyActive = true });

    [HttpPost("deactivate")]
    public async Task<IActionResult> Deactivate() =>
        Ok(new { mode = await emergency.SetAsync("normal"), emergencyActive = false });

    [HttpPost("test")]
    public async Task<IActionResult> Test() =>
        Ok(new { mode = await emergency.SetAsync("test"), emergencyActive = true });
}
