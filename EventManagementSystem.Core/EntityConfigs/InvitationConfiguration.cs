using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
    {
        public void Configure(EntityTypeBuilder<Invitation> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Email)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(i => i.Status)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(i => i.RSVPAt)
                   .IsRequired(false);

            // Event ↔ Invitation (Many-to-One)
            builder.HasOne(i => i.Event)
                   .WithMany() // مفيش ICollection<Invitation> في Event
                   .HasForeignKey(i => i.EventId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
