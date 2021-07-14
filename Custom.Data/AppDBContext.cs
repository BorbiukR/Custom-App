using Custom.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Web.Models
{
    public class AppDBContext : DbContext
    {
        public DbSet<Calculate> Calculates { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options): base(options)
        {
            Database.EnsureCreated();
        }
    }
}