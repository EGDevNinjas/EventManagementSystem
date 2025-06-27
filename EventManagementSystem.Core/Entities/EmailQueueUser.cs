using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class EmailQueueUser
    {
            public int EmailQueueId { get; set; }
            public EmailQueue EmailQueue { get; set; }
            public int UserId { get; set; }
            public User User { get; set; }
    }
}
