using Komuta.Api.Models;
using Komuta.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Komuta.Api.Controllers;

[ApiController]
[Route("api/teams")]
public class TeamsController(TeamService teams) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List() => Ok(await teams.ListAsync());

    [HttpPost]
    public async Task<IActionResult> Create(SaveTeamRequest request)
    {
        try
        {
            return Ok(await teams.CreateAsync(request));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, SaveTeamRequest request)
    {
        try
        {
            return Ok(await teams.UpdateAsync(id, request));
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
            await teams.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
