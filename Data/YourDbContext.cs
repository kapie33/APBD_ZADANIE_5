using Microsoft.EntityFrameworkCore;
using YourNamespace.Models;

namespace YourNamespace.Data
{
    public class YourDbContext : DbContext
    {
        public YourDbContext(DbContextOptions<YourDbContext> options) : base(options)
        {
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientTrip> ClientTrips { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClientTrip>()
                .HasKey(ct => new { ct.ClientId, ct.TripId });

            modelBuilder.Entity<ClientTrip>()
                .HasOne(ct => ct.Client)
                .WithMany(c => c.ClientTrips)
                .HasForeignKey(ct => ct.ClientId);

            modelBuilder.Entity<ClientTrip>()
                .HasOne(ct => ct.Trip)
                .WithMany(t => t.ClientTrips)
                .HasForeignKey(ct => ct.TripId);
        }
    }
}
