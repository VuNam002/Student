using Microsoft.EntityFrameworkCore;
using Student_management.Models.Entities;
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
        public DbSet<Person> Persons { get; set; }

        protected AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>()
                .Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            modelBuilder.Entity<Student>()
                .ToTable(tb => tb.HasTrigger("Student_Trigger"));

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Account)
                .WithOne(a => a.Teacher)
                .HasForeignKey<Teacher>(t => t.AccountID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Teacher>()
                .ToTable(tb => tb.HasTrigger("Teacher_Trigger"));

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Account)
                .WithOne(a => a.Student)
                .HasForeignKey<Student>(s => s.AccountID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .ToTable(tb => tb.HasTrigger("Account_Trigger"));

            modelBuilder.Entity<Person>()
                .ToTable(tb => tb.HasTrigger("Person_Trigger"));

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Person)
                .WithOne(p => p.Student)
                .HasForeignKey<Student>(s => s.PersonID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Person)
                .WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(t => t.PersonID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => rp.RolePermissionID);

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("RolePermission");
            });

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.DepartmentID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Classes)
                .HasForeignKey(c => c.DepartmentID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TeacherID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Class>().ToTable("Class");

            base.OnModelCreating(modelBuilder);
        }
    }
}