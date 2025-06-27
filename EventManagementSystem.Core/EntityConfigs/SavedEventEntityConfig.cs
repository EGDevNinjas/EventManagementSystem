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
    public class SavedEventEntityConfig : IEntityTypeConfiguration<SavedEvent>
    {
        public void Configure(EntityTypeBuilder<SavedEvent> builder)
        {
            builder.HasKey(se => new { se.UserId, se.EventId });

            builder.Property(se => se.CreatedAt)
                .IsRequired();

            builder.HasOne(se => se.User)
                .WithMany(u => u.SavedEvents)
                .HasForeignKey(se => se.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(se => se.Event)
                .WithMany()
                .HasForeignKey(se => se.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
