using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.DTOs
{
    public class AttendanceDto
    {
        public int BookingId { get; set; }
        public string EventTitle { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime CheckInTime { get; set; }
    }
}
