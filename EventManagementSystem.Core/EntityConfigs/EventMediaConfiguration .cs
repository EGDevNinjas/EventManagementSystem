using EventManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventManagementSystem.Core.EntityConfigs
{
    public class EventMediaConfiguration : IEntityTypeConfiguration<EventMedia>
    {
        public void Configure(EntityTypeBuilder<EventMedia> builder)
        {
            builder.HasKey(em => em.Id);

            builder.Property(em => em.FilePath)
                   .IsRequired()
                   .HasMaxLength(300);

            builder.Property(em => em.MediaType)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(em => em.UploadedAt)
                   .IsRequired();

            // العلاقة متضبطة بالفعل من EventConfiguration
            // فمش محتاج تضيف .HasOne() هنا إلا لو حبيت توثق الطرف التاني
        }
    }
}
