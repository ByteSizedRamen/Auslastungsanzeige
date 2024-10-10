using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AuslastungsanzeigeApp.Data;
using AuslastungsanzeigeApp.BusinessLogic;
using AuslastungsanzeigeApp.Api.Models;
using AuslastungsanzeigeApp.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options
 =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),

                     ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));


// Registrierung der Services, Port 8080 ersetzt den default 5000
builder.Services.AddScoped<SensorDataService>();
builder.Services.AddScoped<SensorDataProcessor>();
//builder.WebHost.UseUrls("http://*:8080");
builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(8080); // Sensordaten
    options.ListenAnyIP(8081); // Frontend
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("Schjell",
    builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("Schjell");
app.UseRouting();


// API-Endpoint und Verarbeitung für die Sensordaten
app.MapPost("/sensordata", async (HttpContext context, SensorDataService sensorDataService, SensorDataProcessor sensorDataProcessor) =>
{
    try
    {
        // Liest die JSON aus dem HTTP-Request
        context.Request.EnableBuffering();
        Console.WriteLine(context.Request.QueryString);

        using var reader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8);
        var json = await reader.ReadToEndAsync();

        // Deserialisiert die JSON zu einer SensorDataDto Entity (ohne Speicherung)
        var sensorData = JsonSerializer.Deserialize<SensorDataDto>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });


        // Hat auch alles geklappt wie erwartet?
        if (sensorData == null)
        {
            return Results.BadRequest("Die übermittelten Sensordaten waren nicht valide.");
        }

        await sensorDataService.ProcessSensorDataAsync(sensorData);

        // 
        //await sensorDataService.ProcessSensorData(sensorData);

        if (sensorData.Gewicht != 0)
        {
            Console.WriteLine(sensorData.Gewicht.ToString());

            // Erstelt eine Zug-Entity aus den Sensordaten
            var aktuelleAuslastung = await sensorDataService.ProcessSensorDataAsync(sensorData);

            // Wendet die Business-Logik auf dem neuen Zug an (errechnet eine Auslastung anhand der Personenzahl)
            var auslastung = sensorDataProcessor.ProcessSensorData(aktuelleAuslastung);


        }
        else
        {
            return Results.BadRequest("Verarbeitungsfehler: JSON fehlerhaft");
        }

        return Results.Ok();
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"JSON deserialization error: {ex}");
        return Results.BadRequest($"Invalid JSON format: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.StatusCode(500);
    }
});

// API-Endpoint für Kommunikation mit dem Frontend
app.MapGet("/frontend", async (HttpContext context, SensorDataProcessor sensorDataProcessor, SensorDataService sensorDataService) =>
{

    // Zieht die Sitzplatzbelegung aus der Tabelle raus: Zu Demozwecken wird jeder mitgeschickte Input überschrieben.
    string inputOverride = "Brezel-1";

    try
    {
        var json = sensorDataService.CreateSeatAvailabilityJsonAsync(inputOverride);
        return Results.Ok(json);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
        return Results.StatusCode(500);
    }

});


// Stellt sicher, dass die DB im Kontext angelegt wird
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    context.Database.EnsureCreated();

}

app.Run();