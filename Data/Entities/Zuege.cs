using System.ComponentModel.DataAnnotations.Schema;

namespace AuslastungsanzeigeApp.Data.Entities
{

    [Table("Zuege")]
    public class Zuege
    {
        public int Id { get; set; }
        public string Zugname { get; set; }
        public int Sitze { get; set; }


    }
}