using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.DTOs
{
    public class SavedEventDto
    {
        public int EventId { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public DateTime SavedAt { get; set; } = DateTime.Now;
        public string Location { get; set; }
    }
}
