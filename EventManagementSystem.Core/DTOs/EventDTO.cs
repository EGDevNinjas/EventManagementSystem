namespace EventManagementSystem.Core.DTOs
{
    public class EventDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int OrganizerId { get; set; }
        public DateTime Date { get; set; }
        public int DurationInHours { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int MaxAttendees { get; set; }
        public string Status { get; set; }
        public string CoverImage { get; set; }
    }
}
