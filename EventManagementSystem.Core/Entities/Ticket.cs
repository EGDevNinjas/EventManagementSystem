using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public decimal Price { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }

        public Event Event { get; set; }
        public User User { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }

}
