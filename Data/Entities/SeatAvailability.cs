using System.ComponentModel.DataAnnotations.Schema;

namespace AuslastungsanzeigeApp.Data.Entities
{
public class SeatAvailability
{
    public int Id { get; set; }
    public string Zugname { get; set; }
    
    [NotMapped]
    public string Seats { get; set; }
}
}