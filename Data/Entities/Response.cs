namespace AuslastungsanzeigeApp.Data.Entities 
{
    public class Response
    {
        public int Id { get; set; } // Primary key
        public double Gewicht  { get; set; }
        public int Sitzauslastung { get; set; }
        public string Zugnummer { get; set; }

    }
}