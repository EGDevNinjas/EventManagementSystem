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
    public class SpeakerEntityConfig : IEntityTypeConfiguration<Speaker>
    {
        public void Configure(EntityTypeBuilder<Speaker> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Title)
                .HasMaxLength(100);

            builder.HasMany(s => s.Sessions)
                .WithOne(sess => sess.Speaker)
                .HasForeignKey(sess => sess.SpeakerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(s => s.EventsSpeakers)
                .WithOne(es => es.Speaker)
                .HasForeignKey(es => es.SpeakerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
