using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class EventsSpeakerConfiguration : IEntityTypeConfiguration<EventsSpeaker>
    {
        public void Configure(EntityTypeBuilder<EventsSpeaker> builder)
        {
            // Composite Key
            builder.HasKey(es => new { es.EventId, es.SpeakerId });

            // Event ↔ EventsSpeaker
            builder.HasOne(es => es.Event)
                   .WithMany()
                   .HasForeignKey(es => es.EventId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Speaker ↔ EventsSpeaker
            builder.HasOne(es => es.Speaker)
                   .WithMany(s => s.EventsSpeakers)
                   .HasForeignKey(es => es.SpeakerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
