using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class EventMedia
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string FilePath { get; set; }
        public string MediaType { get; set; }
        public DateTime UploadedAt { get; set; }

        public Event Event { get; set; }
    }
}
