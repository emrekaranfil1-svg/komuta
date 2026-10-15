using Microsoft.EntityFrameworkCore;

namespace Komuta.Api.Data;

public static class DbMigrations
{
    public static void RunAll(KomutaDbContext db)
    {
        db.Database.EnsureCreated();
        try
        {
            db.Database.ExecuteSqlRaw("ALTER TABLE SystemState ADD COLUMN Mode TEXT NOT NULL DEFAULT 'normal'");
        }
        catch
        {
            /* column already exists */
        }

        db.Database.ExecuteSqlRaw(
            "UPDATE SystemState SET Mode = CASE WHEN EmergencyActive = 1 THEN 'emergency' ELSE 'normal' END WHERE Mode IS NULL OR Mode = ''");
    }
}
