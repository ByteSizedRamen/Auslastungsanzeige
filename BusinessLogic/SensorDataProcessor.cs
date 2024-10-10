using AuslastungsanzeigeApp.Data.Entities;
using AuslastungsanzeigeApp.Services;

namespace AuslastungsanzeigeApp.BusinessLogic
{
    public class SensorDataProcessor
    {
        private readonly SensorDataService _sensorDataService;

        public SensorDataProcessor(SensorDataService sensorDataService)
        {
            _sensorDataService = sensorDataService;
        }

        public void ProcessSensorData(Zuege newZug)
        {
            // Businesslogic
            if (newZug.Sitze == 50)
            {
                Console.WriteLine("Gewicht: " + newZug.Sitze.ToString());
                
                
            }

        }
    }
}