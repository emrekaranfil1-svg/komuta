using Komuta.Api.Models;
using Komuta.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Komuta.Api.Controllers;

[ApiController]
[Route("api/personnel")]
public class PersonnelController(PresenceService presence, TeamService teams) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await presence.ListAsync());

    [HttpPost("{id:int}/toggle")]
    public async Task<IActionResult> Toggle(int id, TogglePresenceRequest? request)
    {
        try
        {
            return Ok(await presence.SetInsideAsync(id, request?.Inside, "admin"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/teams")]
    public async Task<IActionResult> Assign(int id, AssignTeamsRequest request)
    {
        try
        {
            await teams.AssignAsync(id, request.TeamIds ?? []);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(SavePersonRequest request)
    {
        try
        {
            return Ok(await presence.CreateAsync(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SavePersonRequest request)
    {
        try
        {
            return Ok(await presence.UpdateAsync(id, request));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await presence.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
