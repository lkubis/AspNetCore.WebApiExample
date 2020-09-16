using AspNetCore.Domain;
using AspNetCore.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Infrastructure
{
    public class DataContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DataContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
        }
    }
}
