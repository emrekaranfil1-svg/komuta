namespace Komuta.Api.Models;

public class UserAccount
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "admin";
}

public class Person
{
    public int Id { get; set; }
    public string CardId { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Title { get; set; } = "";
    public string Kind { get; set; } = "employee";
    public bool IsInside { get; set; }
    public bool IsVip { get; set; }
    public int AvatarSeed { get; set; }
    public DateTime? LastEntryAt { get; set; }
    public DateTime? LastExitAt { get; set; }
    public ICollection<TeamMembership> Memberships { get; set; } = [];
}

public class EmergencyTeam
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public int SortOrder { get; set; }
    public int MinInside { get; set; }
    public bool IsCritical { get; set; }
    public string Accent { get; set; } = "#00e5ff";
    public ICollection<TeamMembership> Memberships { get; set; } = [];
}

public class TeamMembership
{
    public int PersonId { get; set; }
    public int TeamId { get; set; }
    public Person Person { get; set; } = null!;
    public EmergencyTeam Team { get; set; } = null!;
}

public class PresenceEvent
{
    public int Id { get; set; }
    public int PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public bool Entered { get; set; }
    public string Source { get; set; } = "turnstile";
    public DateTime At { get; set; }
}

public class SystemState
{
    public int Id { get; set; }
    public bool EmergencyActive { get; set; }
    public string Mode { get; set; } = "normal";
    public DateTime? EmergencyStartedAt { get; set; }
    public int AccidentFreeDays { get; set; } = 312;
    public DateTime? LastTurnstileAt { get; set; }
    public string? LastTurnstileName { get; set; }
    public bool LastTurnstileEntered { get; set; }
}
