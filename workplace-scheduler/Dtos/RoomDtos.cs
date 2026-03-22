namespace workplace_scheduler.Dtos
{
    public record CreateRoomDto(string Name, string Location, int Capacity);
    public record UpdateRoomDto(string Name, string Location, int Capacity, bool IsActive);
    public record RoomDto(int Id, string Name, string Location, int Capacity, bool IsActive, DateTime CreatedAt);
}
