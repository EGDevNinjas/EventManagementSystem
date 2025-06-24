using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class Organizer
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }
        public string Company { get; set; }
        public bool IsActive { get; set; }

        public User User { get; set; }
        public ICollection<Event> Events { get; set; }
    }
}
