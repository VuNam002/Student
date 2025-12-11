using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Student_management.Models.Entities;

namespace Student_management.Data.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Student");

            builder.HasKey(s => s.StudentID);

            builder.Property(s => s.StudentCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(s => s.StudentCode).IsUnique();

            builder.HasOne(s => s.Person)
                .WithOne(p => p.Student)
                .HasForeignKey<Student>(s => s.PersonID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Account)
                .WithOne(a => a.Student)
                .HasForeignKey<Student>(s => s.AccountID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable(tb => tb.HasTrigger("Student_Trigger"));
        }
    }
}