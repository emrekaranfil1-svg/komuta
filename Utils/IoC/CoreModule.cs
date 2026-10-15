using Microsoft.EntityFrameworkCore;
using Komuta.Api.Abstractions;
using Komuta.Api.Data;
using Komuta.Api.Services;

namespace Komuta.Api.Utils.IoC;

public class CoreModule(IConfiguration configuration) : IModule
{
    public void RegisterServices(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                policy => policy.WithOrigins(
                        "http://localhost:5048",
                        "http://127.0.0.1:5048",
                        "http://localhost:8080",
                        "http://127.0.0.1:8080",
                        "http://localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);

        services.AddOpenApi();

        services.AddDbContext<KomutaDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("KomutaDb") ?? "Data Source=komuta.db"));

        services.AddScoped<DashboardService>();
        services.AddScoped<PresenceService>();
        services.AddScoped<TeamService>();
        services.AddScoped<EmergencyService>();
    }
}
