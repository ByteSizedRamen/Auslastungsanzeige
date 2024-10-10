using AuslastungsanzeigeApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace AuslastungsanzeigeApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
 options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        //public DbSet<Entities.SensorReading> SensorReadings { get; set; }
        public DbSet<Entities.Zuege> Zuege { get; set; }
        public DbSet<Entities.Auslastung> Auslastung { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the NewEntity table to use the 'Id' property as the primary key
            modelBuilder.Entity<Zuege>()
                .HasKey(e => e.Id);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString
 = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            }
        }
    }
}