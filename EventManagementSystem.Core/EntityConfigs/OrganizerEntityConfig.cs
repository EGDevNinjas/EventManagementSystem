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
    public class OrganizerEntityConfig : IEntityTypeConfiguration<Organizer>
    {
        public void Configure(EntityTypeBuilder<Organizer> builder)
        {
            builder.HasKey(o => o.UserId);

            builder.Property(o => o.Company)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(o => o.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.HasOne(o => o.User)
                .WithOne(u => u.Organizer)
                .HasForeignKey<Organizer>(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.Events)
                .WithOne(e => e.Organizer)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
