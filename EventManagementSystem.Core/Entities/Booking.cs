using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string QRCode { get; set; }
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }
        public int? CheckedInByStaffId { get; set; }
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public Ticket Ticket { get; set; }
        public Staff CheckedInByStaff { get; set; }
        public Payment Payment { get; set; }
    }
}
