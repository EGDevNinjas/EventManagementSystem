﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class SavedEvent
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Event Event { get; set; }
    }
}
