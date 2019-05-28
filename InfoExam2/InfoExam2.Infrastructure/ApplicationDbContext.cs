using InfoExam2.Core;
using Microsoft.EntityFrameworkCore;

namespace InfoExam2.Infrastructure
{
	public class ApplicationDbContext : DbContext
	{
		private string _connectionString;

		public ApplicationDbContext()
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Dish> Dishes { get; set; }
		public DbSet<Restaraunt> Restaraunts { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }
		public DbSet<PromoCode> PromoCodes { get; set; }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseInMemoryDatabase("InMemory2");
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
		}
	}
}