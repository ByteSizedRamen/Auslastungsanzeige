namespace AuslastungsanzeigeApp.Data.Entities 
{
    public class Seat
    {
        public string Id { get; set; }
        public bool Taken { get; set; }
    }

    public class SeatData
    {
        public List<Seat> Seats { get; set; }
    }
}