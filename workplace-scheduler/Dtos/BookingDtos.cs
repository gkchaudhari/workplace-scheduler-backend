namespace workplace_scheduler.Dtos
{
    public record CreateBookingDto(int RoomId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime, string Title, int AttendeesCount, string? Agenda);
    public record UpdateBookingDto(int RoomId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime, string Title, int AttendeesCount, string? Agenda, string Status);
    public record BookingDto(int Id, int RoomId, int UserId, DateTime Date, TimeSpan StartTime, TimeSpan EndTime, string Title, int AttendeesCount, string? Agenda, string Status, DateTime CreatedAt);
}
