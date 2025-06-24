using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManagementSystem.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public DateTime? PaidAt { get; set; }

        public Booking Booking { get; set; }
    }
}
