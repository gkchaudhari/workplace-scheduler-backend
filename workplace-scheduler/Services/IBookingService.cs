using workplace_scheduler.Dtos;

namespace workplace_scheduler.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllAsync();
        Task<IEnumerable<BookingDto>> GetForRoomAsync(int roomId, DateTime date);
        Task<BookingDto?> GetByIdAsync(int id);
        Task<BookingDto> CreateAsync(CreateBookingDto dto, int userId);
        Task<BookingDto?> UpdateAsync(int id, UpdateBookingDto dto, int userId);
        Task<bool> CancelAsync(int id, int userId);
    }
}
