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
    public class BookingEntityConfig : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.UserId)
                .IsRequired();

            builder.Property(b => b.TicketId)
                .IsRequired();

            builder.Property(b => b.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(b => b.TotalPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.QRCode)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.IsCheckedIn)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            // علاقات
            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.Ticket)
                .WithMany(t => t.Bookings)
                .HasForeignKey(b => b.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.CheckedInByStaff)
                .WithMany()
                .HasForeignKey(b => b.CheckedInByStaffId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
