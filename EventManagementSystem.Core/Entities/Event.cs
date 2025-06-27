using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace EventManagementSystem.Core.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int OrganizerId { get; set; }
        public DateTime Date { get; set; }
        public int DurationInHours { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int MaxAttendees { get; set; }
        public EventStatus Status { get; set; } // بدل string Status
        public string CoverImage { get; set; }
        public DateTime CreatedAt { get; set; }

        public Organizer Organizer { get; set; }
        public Category Category { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Session> Sessions { get; set; }
        public ICollection<StaffAssignment> StaffAssignments { get; set; }
        public ICollection<EventMedia> Media { get; set; }
        public ICollection<EventRating> Ratings { get; set; }
    }

    // Enum لتعريف حالات الحدث
    public enum EventStatus
    {
        Draft,
        Published,
        Cancelled,
        Completed
    }
}
