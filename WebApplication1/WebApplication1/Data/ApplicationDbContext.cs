using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<ShamsiCalendarEvent> ShamsiCalendarEvents { get; set; }

        public ApplicationDbContext(DbContextOptions options):base(options)
        {
            
        }
    }
}
