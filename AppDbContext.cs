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

        public DbSet<Student> Students { get; set; }

        public DbSet<Instructor> Instructors { get; set; }

        public DbSet<WithdrawalHistory> WithdrawalHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>().HasIndex(s => s.StudentId).IsUnique();
            modelBuilder.Entity<Instructor>().HasIndex(i => i.InstructorId).IsUnique();

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, StudentId = "STU0001", FullName = "John Smith" },
                new Student { Id = 2, StudentId = "STU0002", FullName = "Emily Johnson" },
                new Student { Id = 3, StudentId = "STU0003", FullName = "Michael Brown" },
                new Student { Id = 4, StudentId = "STU0004", FullName = "Sophia Davis" },
                new Student { Id = 5, StudentId = "STU0005", FullName = "Daniel Wilson" },
                new Student { Id = 6, StudentId = "STU0006", FullName = "Olivia Martinez" },
                new Student { Id = 7, StudentId = "STU0007", FullName = "Liam Anderson" },
                new Student { Id = 8, StudentId = "STU0008", FullName = "Ava Thomas" }
            );

            modelBuilder.Entity<Instructor>().HasData(
                new Instructor { Id = 1, InstructorId = "INS0001", FullName = "Sarah Jones" },
                new Instructor { Id = 2, InstructorId = "INS0002", FullName = "David Lee" },
                new Instructor { Id = 3, InstructorId = "INS0003", FullName = "Megan Clark" },
                new Instructor { Id = 4, InstructorId = "INS0004", FullName = "James Walker" },
                new Instructor { Id = 5, InstructorId = "INS0005", FullName = "Rachel Hall" }
            );
        }
    }
}