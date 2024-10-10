namespace AuslastungsanzeigeApp.Data.Entities
{
    public class SensorReading
    {
        public int Id { get; set; } // Primary key
        public string Zugname { get; set; }
        public double Gewicht { get; set; }

    }
}