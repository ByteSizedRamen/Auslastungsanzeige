namespace AuslastungsanzeigeApp.Api.Models
{
    public class SensorDataDto
    {
        public int Id { get; set; }
        public string Zugname { get; set; }
        public double Gewicht { get; set; }
        public int Sitzauslastung { get; set; }
        public string Station { get; set; }


    }
}