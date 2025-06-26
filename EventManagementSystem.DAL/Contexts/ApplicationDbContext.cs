using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using EventManagementSystem.Core.Entities;
using EventManagementSystem.Core.EntityConfigs;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.DAL.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

        // Users & Roles
        public DbSet<User> Users { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Staff> Staffs { get; set; }

        // Events
        public DbSet<Category> Categories { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<EventsSpeaker> EventsSpeakers { get; set; }
        public DbSet<EventMedia> EventMedia { get; set; }

        // Tickets & Bookings
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // User Actions
        public DbSet<SavedEvent> SavedEvents { get; set; }
        public DbSet<EventRating> EventRatings { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Invitation> Invitations { get; set; }

        // Admin / System
        public DbSet<StaffAssignment> StaffAssignments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EmailQueue> EmailQueues { get; set; }
        public DbSet<UserRole> userRoles { get; set; }
        public DbSet<Role> Roles { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntityConfig).Assembly);

            // Composite Keys
            modelBuilder.Entity<SavedEvent>()
                .HasKey(se => new { se.UserId, se.EventId });

            modelBuilder.Entity<EventsSpeaker>()
                .HasKey(es => new { es.EventId, es.SpeakerId });


            modelBuilder.Entity<Ticket>()
                .HasIndex(t => t.EventId);


            // One-to-One: User ↔ Organizer
            modelBuilder.Entity<Organizer>()
                .HasOne(o => o.User)
                .WithOne(u => u.Organizer)
                .HasForeignKey<Organizer>(o => o.Id);

            // One-to-One: User ↔ Staff
            modelBuilder.Entity<Staff>()
                .HasOne(s => s.User)
                .WithOne(u => u.Staff)
                .HasForeignKey<Staff>(s => s.Id);

            // One-to-One: Booking ↔ Payment
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Booking)
                .WithOne(b => b.Payment)
                .HasForeignKey<Payment>(p => p.BookingId);

            modelBuilder.Entity<EventRating>()
            .HasOne(er => er.Event)
            .WithMany(e => e.Ratings)
            .HasForeignKey(er => er.EventId)
            .OnDelete(DeleteBehavior.Cascade); //

            modelBuilder.Entity<EventRating>()
                .HasOne(er => er.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 

            modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);



        }



    }
}
