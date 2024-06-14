using MaiCommerce.Models;
using Microsoft.EntityFrameworkCore;

//Standard Configuration for working with Entity Framework
namespace MaiCommerce.DataAccess.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>().HasData(
                new Category{ Id = 1, Name = "Action", DisplayOrder = 1},
                new Category{ Id = 2, Name = "SciFi", DisplayOrder = 2},
                new Category{ Id = 3, Name = "History", DisplayOrder = 3}
            );
        }
    }
}