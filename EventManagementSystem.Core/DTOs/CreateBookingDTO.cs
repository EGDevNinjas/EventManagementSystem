using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Core.DTOs
{
    public class CreateBookingDTO
    {
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
