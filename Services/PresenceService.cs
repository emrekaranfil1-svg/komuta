using Komuta.Api.Data;
using Komuta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Komuta.Api.Services;

public class PresenceService(KomutaDbContext db)
{
    public async Task<IReadOnlyList<MemberDto>> ListAsync()
    {
        return await db.People.AsNoTracking()
            .Include(p => p.Memberships)
            .OrderBy(p => p.Kind)
            .ThenBy(p => p.FullName)
            .Select(p => new MemberDto(
                p.Id, p.CardId, p.FullName, p.Title, p.Kind, p.IsInside, p.IsVip,
                p.AvatarSeed, p.LastEntryAt, p.Memberships.Select(m => m.TeamId).ToArray()))
            .ToListAsync();
    }

    public async Task<MemberDto> SetInsideAsync(int personId, bool? inside, string source)
    {
        var person = await db.People.Include(p => p.Memberships).FirstOrDefaultAsync(p => p.Id == personId)
            ?? throw new KeyNotFoundException("Personel bulunamadı.");

        var next = inside ?? !person.IsInside;
        return await ApplyAsync(person, next, source);
    }

    public async Task<MemberDto> SwipeAsync(string cardId)
    {
        var person = await db.People.Include(p => p.Memberships)
            .FirstOrDefaultAsync(p => p.CardId == cardId)
            ?? throw new KeyNotFoundException("Kart bulunamadı.");
        return await ApplyAsync(person, !person.IsInside, "turnstile");
    }

    public async Task<MemberDto> RandomSwipeAsync()
    {
        var people = await db.People.Include(p => p.Memberships).ToListAsync();
        var person = people[Random.Shared.Next(people.Count)];
        return await ApplyAsync(person, !person.IsInside, "turnstile");
    }

    private async Task<MemberDto> ApplyAsync(Person person, bool next, string source)
    {
        person.IsInside = next;
        var now = DateTime.Now;
        if (next) person.LastEntryAt = now;
        else person.LastExitAt = now;

        db.PresenceEvents.Add(new PresenceEvent
        {
            PersonId = person.Id,
            Entered = next,
            Source = source,
            At = now
        });

        var state = await db.SystemState.FirstAsync();
        state.LastTurnstileAt = now;
        state.LastTurnstileName = person.FullName;
        state.LastTurnstileEntered = next;

        await db.SaveChangesAsync();

        return ToDto(person);
    }

    public async Task<MemberDto> CreateAsync(SavePersonRequest request)
    {
        var name = (request.FullName ?? "").Trim();
        if (name.Length < 2)
            throw new InvalidOperationException("Ad soyad gerekli.");

        var cardId = await NextCardAsync(request.CardId);
        await EnsureCardAsync(cardId, null);
        var person = new Person
        {
            FullName = name,
            Title = (request.Title ?? "").Trim(),
            Kind = string.IsNullOrWhiteSpace(request.Kind) ? "employee" : request.Kind.Trim(),
            CardId = cardId,
            IsInside = request.IsInside,
            IsVip = request.IsVip,
            AvatarSeed = Random.Shared.Next(1, 98),
            LastEntryAt = request.IsInside ? DateTime.Now : null
        };
        db.People.Add(person);
        await db.SaveChangesAsync();
        await teamsReplace(person.Id, request.TeamIds);
        var saved = await db.People.Include(p => p.Memberships).FirstAsync(p => p.Id == person.Id);
        return ToDto(saved);
    }

    public async Task<MemberDto> UpdateAsync(int id, SavePersonRequest request)
    {
        var person = await db.People.Include(p => p.Memberships).FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException("Personel bulunamadı.");

        var name = (request.FullName ?? "").Trim();
        if (name.Length < 2)
            throw new InvalidOperationException("Ad soyad gerekli.");

        person.FullName = name;
        person.Title = (request.Title ?? "").Trim();
        person.Kind = string.IsNullOrWhiteSpace(request.Kind) ? person.Kind : request.Kind.Trim();
        if (!string.IsNullOrWhiteSpace(request.CardId))
        {
            var cardId = request.CardId.Trim();
            await EnsureCardAsync(cardId, id);
            person.CardId = cardId;
        }
        person.IsVip = request.IsVip;
        if (person.IsInside != request.IsInside)
            await ApplyAsync(person, request.IsInside, "admin");
        else
            await db.SaveChangesAsync();

        await teamsReplace(person.Id, request.TeamIds);
        var saved = await db.People.Include(p => p.Memberships).FirstAsync(p => p.Id == person.Id);
        return ToDto(saved);
    }

    public async Task DeleteAsync(int id)
    {
        var person = await db.People.FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new KeyNotFoundException("Personel bulunamadı.");
        db.People.Remove(person);
        await db.SaveChangesAsync();
    }

    private async Task teamsReplace(int personId, int[]? teamIds)
    {
        var person = await db.People.Include(p => p.Memberships).FirstAsync(p => p.Id == personId);
        db.Memberships.RemoveRange(person.Memberships);
        foreach (var teamId in (teamIds ?? []).Distinct())
            db.Memberships.Add(new TeamMembership { PersonId = personId, TeamId = teamId });
        await db.SaveChangesAsync();
    }

    private async Task EnsureCardAsync(string cardId, int? exceptId)
    {
        var taken = await db.People.AnyAsync(p => p.CardId == cardId && (exceptId == null || p.Id != exceptId));
        if (taken)
            throw new InvalidOperationException("Bu kart numarası zaten kayıtlı.");
    }

    private async Task<string> NextCardAsync(string? requested)
    {
        var card = (requested ?? "").Trim();
        if (card.Length > 0)
            return card;
        var last = await db.People.Select(p => p.CardId).ToListAsync();
        var max = 0;
        foreach (var c in last)
            if (c.StartsWith("BC-") && int.TryParse(c[3..], out var n) && n > max)
                max = n;
        return $"BC-{max + 1:D4}";
    }

    private static MemberDto ToDto(Person person) => new(
        person.Id, person.CardId, person.FullName, person.Title, person.Kind,
        person.IsInside, person.IsVip, person.AvatarSeed, person.LastEntryAt,
        person.Memberships.Select(m => m.TeamId).ToArray());
}
