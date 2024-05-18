using Microsoft.EntityFrameworkCore;

namespace EventInfo.Models
{
    public class Context : DbContext
    {
        public DbSet<PondData> PondData { get; set; } = null!;

        public Context(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PondData>()
                .HasKey(e => e.EntryId);

            modelBuilder
                .Entity<PondData>()
                .Property(e => e.EntryId)
                .ValueGeneratedOnAdd();
        }
    }
}
