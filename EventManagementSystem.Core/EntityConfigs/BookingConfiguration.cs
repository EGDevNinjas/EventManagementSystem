using System.Reflection.Emit;
using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.QRCode)
         .HasColumnType("nvarchar(max)");



            builder.Property(b => b.TotalPrice)
                   .HasColumnType("decimal(10,2)");

            builder.Property(b => b.CreatedAt)
                   .IsRequired();

            // Ticket ↔ Booking (Many-to-One)
            builder.HasOne(b => b.Ticket)
                   .WithMany(t => t.Bookings)
                   .HasForeignKey(b => b.TicketId)
                   .OnDelete(DeleteBehavior.Restrict); // لا تمسح الحجوزات لو التيكت اتمسحت

            // Staff ↔ Booking (Optional Many-to-One)
            builder.HasOne(b => b.CheckedInByStaff)
                   .WithMany()
                   .HasForeignKey(b => b.CheckedInByStaffId)
                   .OnDelete(DeleteBehavior.NoAction); 

            // Booking ↔ Payment (One-to-One)
            builder.HasOne(b => b.Payment)
                   .WithOne(p => p.Booking)
                   .HasForeignKey<Payment>(p => p.BookingId)
                   .OnDelete(DeleteBehavior.NoAction); 
        }
    }
}
