using Microsoft.EntityFrameworkCore;
using DocumentDAL.Data;
using DocumentDAL.Repositories;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDocumentItemRepository, DocumentItemRepository>();
builder.Services.AddScoped<IDocumentDataRepository, DocumentDataRepository>();
builder.Services.AddScoped<IDocumentContentRepository, DocumentContentRepository>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DocumentContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DocumentDatabase")));




var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DocumentContext>();

    try
    {
        Console.WriteLine("Versuche, eine Verbindung zur Datenbank herzustellen...");

        // Warte, bis die Datenbank bereit ist (mit einem maximalen Zeitlimit)
        int retries = 20; // Maximal 50 Sekunden warten
        while (!context.Database.CanConnect() && retries > 0)
        {
            Console.WriteLine("Datenbank ist noch nicht bereit, warte...");
            Thread.Sleep(2500); // Warte 2.5 Sekunden
            retries--;
        }

        if (retries == 0)
        {
            throw new Exception("Datenbank konnte nach mehreren Versuchen nicht erreicht werden.");
        }

        // Migrations anwenden und die Datenbank erstellen/aktualisieren
        context.Database.EnsureCreated(); // Für einfache Entwicklungsszenarien
        //context.Database.Migrate();


        Console.WriteLine("Datenbankmigrationen erfolgreich angewendet.");
        Console.WriteLine("Verbindung zur Datenbank erfolgreich.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fehler bei der Anwendung der Migrationen: {ex.Message}");
    }
}

app.MapControllers();

app.Run();