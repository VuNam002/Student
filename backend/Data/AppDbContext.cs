using Microsoft.EntityFrameworkCore;
using Student_management.Models;
using System.Security;

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
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình mối quan hệ Account - Teacher (1-0 hoặc 1-1)
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Account)
                .WithOne(a => a.Teacher)
                .HasForeignKey<Teacher>(t => t.AccountID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình mối quan hệ Account - Student (1-0 hoặc 1-1)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Account)
                .WithOne(a => a.Student)
                .HasForeignKey<Student>(s => s.AccountID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Cấu hình mối quan hệ Account - Role (Many-to-One)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany()
                .HasForeignKey(a => a.RoleID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình mối quan hệ Role - Permission (Many-to-Many)
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => rp.RolePermissionID);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany()
                .HasForeignKey(rp => rp.RoleID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionID)
                .OnDelete(DeleteBehavior.Cascade);

            // Cấu hình Teacher - Department
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Department)
                .WithMany()
                .HasForeignKey(t => t.DepartmentID)
                .IsRequired(false);

            // Cấu hình Student - Class
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany()
                .HasForeignKey(s => s.ClassID)
                .IsRequired(false);

            // Cấu hình Class - Department
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentID)
                .IsRequired(false);

            // Cấu hình Class - Teacher (GVCN)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.TeacherGVCN)
                .WithMany()
                .HasForeignKey(c => c.TeacherID_GVCN)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Đặt tên bảng
            modelBuilder.Entity<Class>().ToTable("Class");
            modelBuilder.Entity<RolePermission>().ToTable("Role_Permission");

            base.OnModelCreating(modelBuilder);
        }
    }
}