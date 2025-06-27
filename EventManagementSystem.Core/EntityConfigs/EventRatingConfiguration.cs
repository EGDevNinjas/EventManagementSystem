using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class EventRatingConfiguration : IEntityTypeConfiguration<EventRating>
    {
        public void Configure(EntityTypeBuilder<EventRating> builder)
        {
            builder.HasKey(er => er.Id);

            builder.Property(er => er.Rating)
                   .IsRequired();

            builder.Property(er => er.Comment)
                   .HasMaxLength(1000); // اختياري لكن نقيد الطول

            builder.Property(er => er.CreatedAt)
                   .IsRequired();

            // العلاقات متضبطة من الجهة التانية (User & Event)
        }
    }
}
