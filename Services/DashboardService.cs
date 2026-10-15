using Komuta.Api.Data;
using Komuta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Komuta.Api.Services;

public class DashboardService(KomutaDbContext db)
{
    public async Task<DashboardDto> GetAsync()
    {
        var people = await db.People.AsNoTracking()
            .Include(p => p.Memberships)
            .ThenInclude(m => m.Team)
            .ToListAsync();

        var teams = await db.Teams.AsNoTracking()
            .OrderBy(t => t.SortOrder)
            .ToListAsync();

        var state = await db.SystemState.AsNoTracking().FirstAsync();

        var inside = people.Where(p => p.IsInside).ToList();
        var staffInside = inside.Count(p => p.Kind == "employee");
        var otherInside = inside.Count(p => p.Kind != "employee");

        MemberDto ToMember(Person p) => new(
            p.Id,
            p.CardId,
            p.FullName,
            p.Title,
            p.Kind,
            p.IsInside,
            p.IsVip,
            p.AvatarSeed,
            p.LastEntryAt,
            p.Memberships.Select(m => m.TeamId).ToArray());

        var teamCards = teams.Select(team =>
        {
            var members = people
                .Where(p => p.Memberships.Any(m => m.TeamId == team.Id))
                .OrderBy(p => p.FullName)
                .Select(ToMember)
                .ToList();
            var insideCount = members.Count(m => m.IsInside);
            var warn = insideCount < team.MinInside && state.Mode != "test" && state.Mode != "emergency" && !state.EmergencyActive;
            return new TeamCardDto(
                team.Id, team.Code, team.Name, team.SortOrder, team.MinInside,
                team.IsCritical, team.Accent, insideCount, members.Count, warn, members);
        }).ToList();

        var events = await db.PresenceEvents.AsNoTracking()
            .Include(e => e.Person)
            .OrderByDescending(e => e.At)
            .Take(8)
            .Select(e => new EventDto(e.Id, e.Person.FullName, e.Person.Title, e.Entered, e.Source, e.At, e.Person.AvatarSeed))
            .ToListAsync();

        var remaining = inside
            .OrderBy(p => p.Kind == "employee" ? 0 : 1)
            .ThenBy(p => p.FullName)
            .Select(p => new RemainingDto(
                p.Id,
                p.FullName,
                p.Title,
                p.Kind,
                p.AvatarSeed,
                p.Memberships.Select(m => m.Team.Name).ToArray()))
            .ToList();

        var responders = people
            .Where(p => p.IsInside && p.Memberships.Any(m => m.Team.IsCritical))
            .OrderBy(p => p.Memberships.Min(m => m.Team.SortOrder))
            .Select(ToMember)
            .ToList();

        return new DashboardDto(
            new HudDto(staffInside, otherInside, inside.Count),
            state.AccidentFreeDays,
            state.LastTurnstileAt,
            state.LastTurnstileName,
            state.LastTurnstileEntered,
            state.EmergencyActive,
            string.IsNullOrWhiteSpace(state.Mode)
                ? (state.EmergencyActive ? "emergency" : "normal")
                : state.Mode,
            state.EmergencyStartedAt,
            teamCards,
            events,
            remaining,
            responders);
    }
}
