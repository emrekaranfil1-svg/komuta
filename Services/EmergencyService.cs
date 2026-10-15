using Komuta.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Komuta.Api.Services;

public class EmergencyService(KomutaDbContext db)
{
    public async Task<string> SetAsync(string? mode)
    {
        var next = mode is "test" or "emergency" ? mode : "normal";
        var state = await db.SystemState.FirstAsync();
        state.Mode = next;
        state.EmergencyActive = next != "normal";
        state.EmergencyStartedAt = next == "normal" ? null : DateTime.Now;
        await db.SaveChangesAsync();
        return state.Mode;
    }
}
