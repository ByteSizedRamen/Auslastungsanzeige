using AuslastungsanzeigeApp.Data;
using AuslastungsanzeigeApp.Data.Entities;
using AuslastungsanzeigeApp.Api.Models;
using AuslastungsanzeigeApp.BusinessLogic;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Net.Http.Json;

namespace AuslastungsanzeigeApp.Services
{
    public class SensorDataService
    {
        private readonly ApplicationDbContext _dbContext;

        public SensorDataService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task<Auslastung> ProcessSensorDataAsync(SensorDataDto sensorDataDto)
        {
            var zugAusDatenbank = await this.ReturnZugAusDatenbank(sensorDataDto.Zugname);


            // Berechnet die Personenzahl aus dem Gewicht
            var Personenauslastung = sensorDataDto.Gewicht / 100;

            // Berechnet die Sitzauslastung in Prozent
            double zugauslastung = (Convert.ToDouble(sensorDataDto.Sitzauslastung) / Convert.ToDouble(zugAusDatenbank.Sitze)) * 100;

            // Sichert das Ausleseergebnis in der Datenbank
            var berechneteAuslastung = new Auslastung
            {
                Zugname = zugAusDatenbank.Zugname,
                Gewicht = sensorDataDto.Gewicht,
                Personenzahl = Convert.ToInt32(Personenauslastung),
                Sitzauslastung = zugauslastung,
                Zeitstempel = DateTime.Now,
                Station = sensorDataDto.Station

            };

            try
            {
                // Sichert die Entity in der Datenbank
                _dbContext.Auslastung.Add(berechneteAuslastung);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Database update error: {ex}");
                throw; // yeet zurück zur Program.cs
            }

            return berechneteAuslastung;

            //
            //_sensorDataProcessor.ProcessSensorData(newEntity); 
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
