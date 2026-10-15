namespace Komuta.Api.Models;

public record LoginRequest(string Username, string Password);
public record CurrentUserDto(string Username, string Role);

public record TeamInfoDto(int Id, string Code, string Name, int MinInside, bool IsCritical, string Accent);

public record TogglePresenceRequest(bool? Inside);
public record AssignTeamsRequest(int[] TeamIds);
public record SwipeRequest(string CardId);
public record ModeRequest(string Mode);
public record SavePersonRequest(string FullName, string Title, string Kind, string? CardId, bool IsInside, bool IsVip, int[]? TeamIds);
public record SaveTeamRequest(string Name, int MinInside, bool IsCritical);

public record MemberDto(
    int Id,
    string CardId,
    string FullName,
    string Title,
    string Kind,
    bool IsInside,
    bool IsVip,
    int AvatarSeed,
    DateTime? LastEntryAt,
    int[] TeamIds);

public record TeamCardDto(
    int Id,
    string Code,
    string Name,
    int SortOrder,
    int MinInside,
    bool IsCritical,
    string Accent,
    int InsideCount,
    int TotalCount,
    bool Warn,
    IReadOnlyList<MemberDto> Members);

public record HudDto(int InsideStaff, int VisitorsContractors, int TotalInside);
public record EventDto(int Id, string FullName, string Title, bool Entered, string Source, DateTime At, int AvatarSeed);
public record RemainingDto(int Id, string FullName, string Title, string Kind, int AvatarSeed, string[] Teams);

public record DashboardDto(
    HudDto Hud,
    int AccidentFreeDays,
    DateTime? LastTurnstileAt,
    string? LastTurnstileName,
    bool LastTurnstileEntered,
    bool EmergencyActive,
    string Mode,
    DateTime? EmergencyStartedAt,
    IReadOnlyList<TeamCardDto> Teams,
    IReadOnlyList<EventDto> RecentEvents,
    IReadOnlyList<RemainingDto> RemainingInside,
    IReadOnlyList<MemberDto> RespondersInside);
