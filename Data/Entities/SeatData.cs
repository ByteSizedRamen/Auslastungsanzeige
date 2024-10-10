namespace AuslastungsanzeigeApp.Data.Entities 
{
    public class Seat
    {
        public string id { get; set; }
        public bool taken { get; set; }
    }

    public class SeatData
    {
        public List<Seat> Seats { get; set; }
    }
}