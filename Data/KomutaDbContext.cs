using Komuta.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Komuta.Api.Data;

public sealed class KomutaDbContext(DbContextOptions<KomutaDbContext> options) : DbContext(options)
{
    public DbSet<UserAccount> Users => Set<UserAccount>();
    public DbSet<Person> People => Set<Person>();
    public DbSet<EmergencyTeam> Teams => Set<EmergencyTeam>();
    public DbSet<TeamMembership> Memberships => Set<TeamMembership>();
    public DbSet<PresenceEvent> PresenceEvents => Set<PresenceEvent>();
    public DbSet<SystemState> SystemState => Set<SystemState>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserAccount>()
            .HasIndex(x => x.Username)
            .IsUnique();

        modelBuilder.Entity<Person>()
            .HasIndex(x => x.CardId)
            .IsUnique();

        modelBuilder.Entity<EmergencyTeam>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<TeamMembership>()
            .HasKey(x => new { x.PersonId, x.TeamId });

        modelBuilder.Entity<TeamMembership>()
            .HasOne(x => x.Person)
            .WithMany(x => x.Memberships)
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TeamMembership>()
            .HasOne(x => x.Team)
            .WithMany(x => x.Memberships)
            .HasForeignKey(x => x.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PresenceEvent>()
            .HasOne(x => x.Person)
            .WithMany()
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
