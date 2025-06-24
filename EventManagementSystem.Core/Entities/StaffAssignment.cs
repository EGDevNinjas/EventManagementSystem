using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class StaffAssignment
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public int StaffId { get; set; }
        public string RoleDescription { get; set; }

        public Event Event { get; set; }
        public Staff Staff { get; set; }
    }
}
