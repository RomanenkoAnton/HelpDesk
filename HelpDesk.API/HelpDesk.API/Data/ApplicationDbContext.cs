using HelpDesk.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<LocalUser> LocalUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>().HasData(
                new Department()
                {
                    Id = 1,
                    Title = "Рота Звя'зку"
                },
                new Department()
                {
                    Id = 2,
                    Title = "Рота Охорони"
                },
                new Department()
                {
                    Id = 3,
                    Title = "Рота Підтримки"
                });


        }
    }
}
