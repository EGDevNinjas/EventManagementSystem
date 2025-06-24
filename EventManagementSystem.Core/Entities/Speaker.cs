using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class Speaker
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public ICollection<Session> Sessions { get; set; }
        public ICollection<EventsSpeaker> EventsSpeakers { get; set; }
    }

}
