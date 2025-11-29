using LinenManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LinenManagement.Context
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration config) : base(options) 
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config.GetConnectionString("DbConnection"));
            }
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<CartLog> CartLog { get; set; }
        public DbSet<Carts> Carts { get; set; }
        public DbSet<Locations> Locations { get; set; }
        public DbSet<Linen> Linen { get; set; }
        public DbSet<CartLogDetail> CartLogDetail { get; set; }
    }
}
