using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Student_management.Models.Entities;

namespace Student_management.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Department");

            builder.HasKey(d => d.DepartmentID);

            builder.Property(d => d.DepartmentName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.DepartmentCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(d => d.DepartmentCode).IsUnique();
        }
    }
}