using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Student_management.Models.Entities;

namespace Student_management.Data.Configurations
{
    public class TeacherConfiguration : IEntityTypeConfiguration<Teacher>
    {
        public void Configure(EntityTypeBuilder<Teacher> builder)
        {
            builder.ToTable("Teacher");

            builder.HasKey(t => t.TeacherID);

            builder.Property(t => t.TeacherCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(t => t.TeacherCode).IsUnique();

            builder.HasOne(t => t.Person)
                .WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(t => t.PersonID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Department)
                .WithMany(d => d.Teachers)
                .HasForeignKey(t => t.DepartmentID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Account)
                .WithOne(a => a.Teacher)
                .HasForeignKey<Teacher>(t => t.AccountID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable(tb => tb.HasTrigger("Teacher_Trigger"));
        }
    }
}