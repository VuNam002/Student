using Microsoft.EntityFrameworkCore;
using Student_management.Models;

namespace Student_management.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Class> Classes { get; set; } 
        public DbSet<Role> Roles { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        protected AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Teacher)
                .WithOne(t => t.Account)
                .HasForeignKey<Teacher>(t => t.TaiKhoanID);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Student)
                .WithOne(s => s.Account)
                .HasForeignKey<Student>(s => s.TaiKhoanID);
            modelBuilder.Entity<Class>().ToTable("Class");

            base.OnModelCreating(modelBuilder);
        }
    }
}