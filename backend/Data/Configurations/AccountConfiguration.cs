using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Student_management.Models.Entities;

namespace Student_management.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");

            builder.HasKey(a => a.AccountID);

            builder.Property(a => a.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Password)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(a => a.Email).IsUnique();

            builder.HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(tb => tb.HasTrigger("Account_Trigger"));
        }
    }
}