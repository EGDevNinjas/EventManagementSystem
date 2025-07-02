using System.ComponentModel.DataAnnotations;

namespace EventManagementSystem.Core.DTOs
{
    public class ProcessPaymentDTO
    {
        [Required(ErrorMessage = "Booking ID is required")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transaction ID is required")]
        public string TransactionId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }
    }
}