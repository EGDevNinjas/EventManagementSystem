using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class AuditLogConfig : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.UserId)
                .IsRequired();

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.TargetEntity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.TargetId)
                .IsRequired();

            builder.Property(a => a.Timestamp)
                .IsRequired();

            builder.Property(a => a.Details)
                .IsRequired()
                .HasMaxLength(500);

            // Relationship
            builder.HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
