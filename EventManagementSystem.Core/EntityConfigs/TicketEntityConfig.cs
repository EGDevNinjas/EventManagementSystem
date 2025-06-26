using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class TicketEntityConfig : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> modelBuilder)
        {
            // Primary Key
            modelBuilder.HasKey(t => t.Id);

            // Required Properties
            modelBuilder.Property(t => t.EventId)
                .IsRequired();

            modelBuilder.Property(t => t.UserId)
                .IsRequired();

            modelBuilder.Property(t => t.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasPrecision(18, 2);

            modelBuilder.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Property(t => t.Quantity)
                .IsRequired()
                .HasDefaultValue(1);

            // Constraints
            modelBuilder.Property(t => t.Price)
                .HasAnnotation("Range", new[] { 0, 999999.99 });

            modelBuilder.Property(t => t.Quantity)
                .HasAnnotation("Range", new[] { 1, 1000 });

            // Indexes for better performance
            modelBuilder.HasIndex(t => t.EventId);
            modelBuilder.HasIndex(t => t.UserId);

            // Relationships
            modelBuilder.HasOne(t => t.Event)
                .WithMany(e => e.Tickets)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasMany(t => t.Bookings)
                .WithOne(b => b.Ticket)
                .HasForeignKey(b => b.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
