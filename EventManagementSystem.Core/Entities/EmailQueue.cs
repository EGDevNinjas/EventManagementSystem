using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class EmailQueue
    {
        public int Id { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsSent { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
