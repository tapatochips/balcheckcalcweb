using Microsoft.EntityFrameworkCore;
using balcheckcalcweb.Models;

namespace balcheckcalcweb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CheckHistory> CheckHistories { get; set; }
        public DbSet<CheckHistoryDetail> CheckHistoryDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CheckHistoryDetail>()
                .HasOne(d => d.CheckHistory)
                .WithMany(h => h.PolicyDetails)
                .HasForeignKey(d => d.CheckHistoryId);
        }
    }
}