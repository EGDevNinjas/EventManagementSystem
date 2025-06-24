using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; }
        public string TargetEntity { get; set; }
        public int TargetId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Details { get; set; }

        public User User { get; set; }
    }
}
