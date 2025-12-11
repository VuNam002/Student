using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Student_management.Models.Entities;

namespace Student_management.Data.Configurations
{
    public class ClassConfiguration : IEntityTypeConfiguration<Class>
    {
        public void Configure(EntityTypeBuilder<Class> builder)
        {
            builder.ToTable("Class");

            builder.HasKey(c => c.ClassID);

            builder.Property(c => c.ClassName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ClassCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(c => c.ClassCode).IsUnique();

            builder.HasOne(c => c.Department)
                .WithMany(d => d.Classes)
                .HasForeignKey(c => c.DepartmentID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Teacher)
                .WithMany(t => t.Classes)
                .HasForeignKey(c => c.TeacherID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}