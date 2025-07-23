using EventManagementSystem.Core.Entities;
using EventManagementSystem.DAL.Contexts; // Adjust namespace if different
using Microsoft.EntityFrameworkCore;
using System;

namespace EventManagementSystem.test
{
    internal class InMemoryDbContext : ApplicationDbContext
    {
        public InMemoryDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call base if it has configurations

            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Title = "Test Event 1",
                    Description = "A sample event",
                    CategoryId = 1,
                    OrganizerId = 1,
                    Date = DateTime.Now,
                    DurationInHours = 2,
                    Location = "Test Location 1",
                    Latitude = 40.7128m,
                    Longitude = -74.0060m,
                    MaxAttendees = 100,
                    Status = EventStatus.Published,
                    CoverImage = "image1.jpg",
                    CreatedAt = DateTime.Now
                },
                new Event
                {
                    Id = 2,
                    Title = "Test Event 2",
                    Description = "Another sample event",
                    CategoryId = 2,
                    OrganizerId = 2,
                    Date = DateTime.Now.AddDays(1),
                    DurationInHours = 3,
                    Location = "Test Location 2",
                    Latitude = 34.0522m,
                    Longitude = -118.2437m,
                    MaxAttendees = 150,
                    Status = EventStatus.Draft,
                    CoverImage = "image2.jpg",
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}