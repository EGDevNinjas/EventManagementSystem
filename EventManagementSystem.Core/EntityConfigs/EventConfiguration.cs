using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Description)
                   .HasMaxLength(1000);

            builder.Property(e => e.Location)
                   .HasMaxLength(200);

            builder.Property(e => e.CoverImage)
                   .HasMaxLength(500);

            builder.Property(e => e.Status)
                   .HasMaxLength(50);

            builder.Property(e => e.Latitude)
                   .HasColumnType("decimal(9,6)");

            builder.Property(e => e.Longitude)
                   .HasColumnType("decimal(9,6)");

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            // Category ↔ Event
            builder.HasOne(e => e.Category)
                   .WithMany(c => c.Events)
                   .HasForeignKey(e => e.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Organizer ↔ Event
            builder.HasOne(e => e.Organizer)
                   .WithMany(o => o.Events)
                   .HasForeignKey(e => e.OrganizerId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Event ↔ Ticket
            builder.HasMany(e => e.Tickets)
                   .WithOne(t => t.Event)
                   .HasForeignKey(t => t.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Event ↔ Session
            builder.HasMany(e => e.Sessions)
                   .WithOne(s => s.Event)
                   .HasForeignKey(s => s.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Event ↔ StaffAssignment
            builder.HasMany(e => e.StaffAssignments)
                   .WithOne(sa => sa.Event)
                   .HasForeignKey(sa => sa.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Event ↔ Media
            builder.HasMany(e => e.Media)
                   .WithOne(m => m.Event)
                   .HasForeignKey(m => m.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Event ↔ Ratings
            builder.HasMany(e => e.Ratings)
                   .WithOne(r => r.Event)
                   .HasForeignKey(r => r.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
