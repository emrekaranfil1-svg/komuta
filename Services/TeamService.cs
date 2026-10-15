using System.Linq;
using Komuta.Api.Data;
using Komuta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Komuta.Api.Services;

public class TeamService(KomutaDbContext db)
{
    public async Task<IReadOnlyList<TeamInfoDto>> ListAsync()
    {
        return await db.Teams.AsNoTracking()
            .OrderBy(t => t.SortOrder)
            .Select(t => new TeamInfoDto(t.Id, t.Code, t.Name, t.MinInside, t.IsCritical, t.Accent))
            .ToListAsync();
    }

    public async Task<TeamInfoDto> CreateAsync(SaveTeamRequest request)
    {
        var name = (request.Name ?? "").Trim();
        if (name.Length < 2)
            throw new InvalidOperationException("Ekip adı gerekli.");

        var sort = await db.Teams.Select(t => t.SortOrder).DefaultIfEmpty().MaxAsync() + 1;
        var team = new EmergencyTeam
        {
            Name = name,
            Code = await UniqueCodeAsync(name),
            MinInside = request.MinInside < 0 ? 0 : request.MinInside,
            IsCritical = request.IsCritical,
            Accent = "#0057B8",
            SortOrder = sort
        };
        db.Teams.Add(team);
        await db.SaveChangesAsync();
        return new TeamInfoDto(team.Id, team.Code, team.Name, team.MinInside, team.IsCritical, team.Accent);
    }

    public async Task<TeamInfoDto> UpdateAsync(int id, SaveTeamRequest request)
    {
        var team = await db.Teams.FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new KeyNotFoundException("Ekip bulunamadı.");
        var name = (request.Name ?? "").Trim();
        if (name.Length < 2)
            throw new InvalidOperationException("Ekip adı gerekli.");

        team.Name = name;
        team.MinInside = request.MinInside < 0 ? 0 : request.MinInside;
        team.IsCritical = request.IsCritical;
        await db.SaveChangesAsync();
        return new TeamInfoDto(team.Id, team.Code, team.Name, team.MinInside, team.IsCritical, team.Accent);
    }

    public async Task DeleteAsync(int id)
    {
        var team = await db.Teams.FirstOrDefaultAsync(t => t.Id == id)
            ?? throw new KeyNotFoundException("Ekip bulunamadı.");
        db.Teams.Remove(team);
        await db.SaveChangesAsync();
    }

    private async Task<string> UniqueCodeAsync(string name)
    {
        var map = new Dictionary<char, string>
        {
            ['ç'] = "c", ['ğ'] = "g", ['ı'] = "i", ['ö'] = "o", ['ş'] = "s", ['ü'] = "u",
            ['Ç'] = "c", ['Ğ'] = "g", ['İ'] = "i", ['Ö'] = "o", ['Ş'] = "s", ['Ü'] = "u"
        };
        var raw = string.Concat(name.Select(c => map.TryGetValue(c, out var r) ? r : c.ToString()));
        var slug = new string(raw.ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
        if (string.IsNullOrWhiteSpace(slug)) slug = "ekip";
        var code = slug;
        var i = 2;
        while (await db.Teams.AnyAsync(t => t.Code == code))
            code = $"{slug}{i++}";
        return code;
    }

    public async Task AssignAsync(int personId, int[] teamIds)
    {
        var person = await db.People.Include(p => p.Memberships).FirstOrDefaultAsync(p => p.Id == personId)
            ?? throw new KeyNotFoundException("Personel bulunamadı.");

        db.Memberships.RemoveRange(person.Memberships);
        foreach (var teamId in teamIds.Distinct())
            db.Memberships.Add(new TeamMembership { PersonId = personId, TeamId = teamId });

        await db.SaveChangesAsync();
    }
}
