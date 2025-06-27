using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.TargetEntity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Details)
                .HasMaxLength(1000);

            builder.Property(a => a.Timestamp)
                .IsRequired();

            // Relationship: AuditLog ↔ User (Many-to-One)
            builder.HasOne(a => a.User)
                .WithMany() // لأنه مفيش ICollection<AuditLog> في User
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
