using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class Invitation
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public DateTime? RSVPAt { get; set; }

        public Event Event { get; set; }
    }
}
