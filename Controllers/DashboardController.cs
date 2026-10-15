using Komuta.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Komuta.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController(DashboardService dashboard) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await dashboard.GetAsync());
}
