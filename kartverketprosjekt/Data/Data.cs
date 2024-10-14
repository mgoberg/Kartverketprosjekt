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

        public DbSet<BrukerModel> Bruker { get; set; }
        public DbSet<SakModel> Sak { get; set; }
    }
}