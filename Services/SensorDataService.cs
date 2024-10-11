using AuslastungsanzeigeApp.Data;
using AuslastungsanzeigeApp.Data.Entities;
using AuslastungsanzeigeApp.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace AuslastungsanzeigeApp.Services
{
    public class SensorDataService
    {
        private readonly ApplicationDbContext _dbContext;

        public SensorDataService(ApplicationDbContext dbContext1)
        {
            _dbContext = dbContext1;
        }

        public async Task<Zuege> ReturnZugAusDatenbank(string zugnummer)
        {
            var zugAusDatenbank = await _dbContext.Zuege.FirstOrDefaultAsync(e => e.Zugname == zugnummer);

            if (zugAusDatenbank == null)
            {
                return null;
            }

            return zugAusDatenbank;
        }

        public async Task<string> CreateSeatAvailabilityJsonAsync(string zugname)
        {
            try
            {
                //
                var seatAvailability = await _dbContext.SeatAvailability
                    .FirstOrDefaultAsync(sa => sa.Zugname == zugname);

                if (seatAvailability == null)
                {
                    Console.WriteLine($"Verarbeitungfehler: Zum Zug '{zugname}' wurde keine SeatMap gefunden.");
                    return string.Empty;
                }

                var seatData = seatAvailability.Seats;

                string jsonString = JsonConvert.SerializeObject(seatData, Formatting.Indented);

                return jsonString;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unbehandelte Ausnahme: {ex}");
                return string.Empty;
            }
        }

        public async Task<Auslastung> ProcessSensorDataAsync(SensorDataDto sensorDataDto)
        {
            var zugAusDatenbank = await this.ReturnZugAusDatenbank(sensorDataDto.Zugname);

            // Berechnet die Personenzahl aus dem Gewicht
            var Personenauslastung = sensorDataDto.Gewicht / 100;

            // Berechnet die Sitzauslastung in Prozent
            double zugauslastung = (Convert.ToDouble(sensorDataDto.Sitzauslastung) / Convert.ToDouble(zugAusDatenbank.Sitze)) * 100;

            // Entity für die Auslastung
            var berechneteAuslastung = new Auslastung
            {
                Zugname = zugAusDatenbank.Zugname,
                Gewicht = sensorDataDto.Gewicht,
                Personenzahl = Convert.ToInt32(Personenauslastung),
                Sitzauslastung = zugauslastung,
                Zeitstempel = DateTime.Now,
                Station = sensorDataDto.Station

            };

            // Entity für die SeatAvailability
            var seatData = new SeatData
            {
                Seats = new List<Seat>
                {
                    new Seat { id = "A1", taken = true },
                    new Seat { id = "A2", taken = false },
                    new Seat { id = "A3", taken = true },
                    new Seat { id = "A4", taken = false }

                    // Test
                }
            };

            // Dummydaten für Demonstrationszwecke
            var jsonString = System.Text.Json.JsonSerializer.Serialize(seatData);

            var seatAvailability = new SeatAvailability
            {
                Zugname = sensorDataDto.Zugname,
                Seats = jsonString
            };


            try
            {
                // Sichert die Entities in der Datenbank
                _dbContext.Auslastung.Add(berechneteAuslastung);

                await _dbContext.SaveChangesAsync();

                _dbContext.SeatAvailability.Add(seatAvailability);
                _dbContext.SeatAvailability.Add(seatAvailability);
                await _dbContext.SaveChangesAsync();

                return berechneteAuslastung;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update error: {ex}");
                throw; // yeet zurück zur Program.cs
            }
        }

        public async Task<string> CreateJsonFromEntityAsync(Auslastung auslastungEntity, double auslastungWert)
        {

            if (auslastungWert == null)
            {
                return string.Empty;
            }

            // Erstellt ein Objekt mit den Datenfeldern
            var dataForJson = new
            {
                Auslastung = auslastungWert,
                Personenzahl = auslastungEntity.Personenzahl,
            };

            // Serialisiert die JSON aus dem Objekt
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = System.Text.Json.JsonSerializer.Serialize(dataForJson, options);

            return json;
        }

        public async Task<List<Zuege>> GetNewEntityDataAsync()
        {
            return await _dbContext.Zuege.ToListAsync();
        }


        public async Task SaveSensorDataAsync(SensorDataDto sensorDataDto)
        {
            // 1. Map the DTO to the Entity
            var sensorReading = new SensorReading
            {
                Gewicht = sensorDataDto.Gewicht,
                Zugname = sensorDataDto.Zugname
            };

        }
    }
}
