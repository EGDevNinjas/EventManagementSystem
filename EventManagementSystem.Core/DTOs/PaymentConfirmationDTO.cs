namespace EventManagementSystem.Core.DTOs
{
    public class PaymentConfirmationDTO
    {
        public string Message { get; set; } = string.Empty;
        public PaymentDTO Payment { get; set; } = null!;
        public DateTime ConfirmedAt { get; set; }
    }
}