using System.Reflection.Metadata.Ecma335;
using AuslastungsanzeigeApp.Data.Entities;
using AuslastungsanzeigeApp.Services;
using Microsoft.EntityFrameworkCore;

namespace AuslastungsanzeigeApp.BusinessLogic
{
    public class SensorDataProcessor
    {
        private readonly SensorDataService _sensorDataService;

        public SensorDataProcessor(SensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }

        public double ProcessSensorData(Auslastung aktuelleAuslastung)
        {
            double auslastung = 0;

            // Zieht den Zug zur Zugnummer aus der Datenbank
            Task<Zuege> task = _sensorDataService.ReturnZugAusDatenbank(aktuelleAuslastung.Zugname);

            task.ContinueWith(t =>
    {
        if (t.IsFaulted)
        {
            Console.WriteLine($"Error: {t.Exception.Message}");
            // 
        }
        else if (t.IsCompletedSuccessfully)
        {
            var zugAusDatenbank = t.Result;
        }
    });

            // temporäres Stand-In für die Tabelle
            double maximalePersonenzahl = 10;

            auslastung = (aktuelleAuslastung.Personenzahl / maximalePersonenzahl) * 100;
            
            return auslastung;
        }



    }

}

