using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class EventsSpeaker
    {
        public int SpeakerId { get; set; }
        public int EventId { get; set; }

        public Speaker Speaker { get; set; }
        public Event Event { get; set; }
    }

}
