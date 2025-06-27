using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class EmailQueueConfiguration : IEntityTypeConfiguration<EmailQueue>
    {
        public void Configure(EntityTypeBuilder<EmailQueue> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ToEmail)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(e => e.Subject)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Body)
                   .IsRequired();

            builder.Property(e => e.IsSent)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.SentAt)
                   .IsRequired(false);

            // ممكن نضيف Index لتحسين الأداء
            builder.HasIndex(e => e.IsSent); // علشان لما نعمل SELECT للإيميلات اللي لسه مبعتتش
        }
    }
}
