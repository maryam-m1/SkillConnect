using Microsoft.EntityFrameworkCore;
using SkillConnect.Models;

namespace SkillConnect.Data
{
    public class AppDbContext : DbContext
    {
        // Keep your existing constructor for the website
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ADD THIS: A blank constructor for the migration tool
        public AppDbContext() { }

        // ADD THIS: Backup configuration for the migration tool
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // This string matches your appsettings.json
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SkillConnectDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<QuizResult> QuizResults { get; set; }
    }
}