using workplace_scheduler.Dtos;

namespace workplace_scheduler.Services
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllAsync();
        Task<RoomDto?> GetByIdAsync(int id);
        Task<RoomDto> CreateAsync(CreateRoomDto dto);
        Task<RoomDto?> UpdateAsync(int id, UpdateRoomDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
