using Microsoft.EntityFrameworkCore;
using kartverketprosjekt.Models;

namespace kartverketprosjekt.Data
{
    public class KartverketDbContext : DbContext
    {
        public KartverketDbContext(DbContextOptions<KartverketDbContext> options)
            : base(options)
        {
        }

        public DbSet<BrukerModel> Brukere { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BrukerModel>()
                .HasKey(b => b.BrukerId); // Specify 'Id' as the primary key

            modelBuilder.Entity<BrukerModel>()
                .Property(b => b.BrukerId)
                .ValueGeneratedOnAdd();
        }
    }
}