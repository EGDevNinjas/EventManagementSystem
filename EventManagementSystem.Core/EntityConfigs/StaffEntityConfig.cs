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
    public class StaffEntityConfig : IEntityTypeConfiguration<Staff>
    {
        public void Configure(EntityTypeBuilder<Staff> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.Assignments)
                .WithOne(a => a.Staff)
                .HasForeignKey(a => a.StaffId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
