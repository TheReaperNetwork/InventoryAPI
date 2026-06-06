using Microsoft.EntityFrameworkCore;
using InventoryApi.Models;

namespace InventoryApi
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Purchase> Purchases { get; set; }

        public DbSet<Withdrawal> Withdrawals { get; set; }
    }
}