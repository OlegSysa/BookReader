using BookReader.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookReader.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Translation> Translations => Set<Translation>();
    }
}
