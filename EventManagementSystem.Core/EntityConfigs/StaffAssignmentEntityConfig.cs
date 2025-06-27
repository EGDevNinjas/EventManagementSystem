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
    public class StaffAssignmentEntityConfig : IEntityTypeConfiguration<StaffAssignment>
    {
        public void Configure(EntityTypeBuilder<StaffAssignment> builder)
        {
            builder.HasKey(sa => sa.Id);

            builder.Property(sa => sa.EventId)
                .IsRequired();

            builder.Property(sa => sa.StaffId)
                .IsRequired();

            builder.Property(sa => sa.RoleDescription)
                .HasMaxLength(100);

            builder.HasOne(sa => sa.Event)
                .WithMany(e => e.StaffAssignments)
                .HasForeignKey(sa => sa.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sa => sa.Staff)
                .WithMany(s => s.Assignments)
                .HasForeignKey(sa => sa.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
