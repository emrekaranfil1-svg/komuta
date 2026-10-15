using Komuta.Api.Data;
using Komuta.Api.Utils.Extensions;
using Komuta.Api.Utils.IoC;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDependencyResolvers([new CoreModule(builder.Configuration)]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors("AllowFrontend");
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KomutaDbContext>();
    DbMigrations.RunAll(db);
    DbSeeder.Seed(db);
}

app.Run();
