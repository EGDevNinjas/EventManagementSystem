using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class Staff
    {
        [Key, ForeignKey("User")]
        public int Id { get; set; }
        public string Role { get; set; }

        public User User { get; set; }
        public ICollection<StaffAssignment> Assignments { get; set; }
    }
}
