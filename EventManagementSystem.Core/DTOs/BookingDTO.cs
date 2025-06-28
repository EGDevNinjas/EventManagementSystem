namespace EventManagementSystem.Core.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TicketId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? QRCode { get; set; }
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckInTime { get; set; }
        public int? CheckedInByStaffId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
