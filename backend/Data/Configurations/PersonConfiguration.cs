using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Student_management.Models.Entities;

namespace Student_management.Data.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Person");

            builder.HasKey(p => p.PersonID);

            builder.Property(p => p.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Email)
                .HasMaxLength(100);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(p => p.Gender)
                .HasMaxLength(10);

            builder.Property(p => p.PersonType)
                .HasMaxLength(20);

            builder.HasIndex(p => p.Email);
            builder.HasIndex(p => p.IdentityCard).IsUnique();

            builder.ToTable(tb => tb.HasTrigger("Person_Trigger"));
        }
    }
}