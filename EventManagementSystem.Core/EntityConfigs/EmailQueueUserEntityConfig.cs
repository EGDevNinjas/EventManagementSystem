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
    public class EmailQueueUserEntityConfig : IEntityTypeConfiguration<EmailQueueUser>
    {
        public void Configure(EntityTypeBuilder<EmailQueueUser> builder)
        {
            // Composite Key
            builder.HasKey(eu => new { eu.EmailQueueId, eu.UserId });

            // Relationships
            builder.HasOne(eu => eu.EmailQueue)
                .WithMany(eq => eq.EmailQueueUsers)
                .HasForeignKey(eu => eu.EmailQueueId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(eu => eu.User)
                .WithMany(u => u.EmailQueueUsers)
                .HasForeignKey(eu => eu.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
