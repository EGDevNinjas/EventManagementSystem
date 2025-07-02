using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.DTOs
{
    public class UpdateBookingDTO
    {
        public bool IsCheckedIn { get; set; }
        public int? CheckedInByStaffId { get; set; }
    }
}
