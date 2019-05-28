using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore;
using InfoExam1.Core;

namespace InfoExam1.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        private string _connectionString;
        
        public DbSet<File> Files { get; set; }
        public ApplicationDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("InMemory");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}