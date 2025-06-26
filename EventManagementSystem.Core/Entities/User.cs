using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public bool IsActive { get; set; }
        public int Role { get; set; }
        public Organizer Organizer { get; set; }
        public Staff Staff { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public ICollection<EventRating> Ratings { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<SavedEvent> SavedEvents { get; set; }

    }

}
