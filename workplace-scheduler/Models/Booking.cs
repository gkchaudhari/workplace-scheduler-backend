namespace workplace_scheduler.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Title { get; set; } = null!;
        public string? Agenda { get; set; }
        public int AttendeesCount { get; set; }
        public string Status { get; set; } = "active"; // active, upcoming, completed, cancelled
        public DateTime CreatedAt { get; set; }
    }
}
