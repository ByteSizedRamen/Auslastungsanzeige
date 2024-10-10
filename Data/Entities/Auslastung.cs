namespace AuslastungsanzeigeApp.Data.Entities 
{
    public class Auslastung
    {
        public int Id { get; set; } // Primary key
        public string Zugname { get; set; }
        public double Sitzauslastung { get; set; }
        public double Gewicht { get; set; }
        public int Personenzahl { get; set; }
        public DateTime Zeitstempel { get; set; }
        public string Station { get; set; }

    }
}