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
    public class UserEntityConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.LastName)
                .IsRequired().HasMaxLength(50);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(100);

            // help in faster search + prevent repeat mail
            builder.HasIndex(x => x.Email).IsUnique();

            builder.Property(x => x.Phone)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

            builder.Property(u => u.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(100);

            builder.Property(u => u.Role)
                 .IsRequired();

            // Configuring the relationship with other Entities
            // 1:1 User ↔ Organizer
            builder.HasOne(u => u.Organizer)
           .WithOne(o => o.User)
           .HasForeignKey<Organizer>(o => o.Id);

            // 1:1 User ↔ Staff
            builder.HasOne(u => u.Staff)
            .WithOne(s => s.User)
            .HasForeignKey<Staff>(s => s.Id)
            .OnDelete(DeleteBehavior.Cascade);

            // 1:M User ↔ Bookings
            builder.HasMany(u => u.Bookings)
             .WithOne(b => b.User)
             .HasForeignKey(b => b.UserId)
             .OnDelete(DeleteBehavior.Cascade);
            // 1:M User ↔ Ratings
            builder.HasMany(u => u.Ratings)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            // 1:M User ↔ Notifications
            builder.HasMany(u => u.Notifications)
             .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            // 1:M User ↔ SavedEvents
            builder.HasMany(u => u.SavedEvents)
          .WithOne(se => se.User)
            .HasForeignKey(se => se.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        }
   
    }
}
