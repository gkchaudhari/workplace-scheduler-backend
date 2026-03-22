using Microsoft.EntityFrameworkCore;
using workplace_scheduler.Data;
using workplace_scheduler.Dtos;
using workplace_scheduler.Models;

namespace workplace_scheduler.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _db;

        public BookingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<BookingDto>> GetAllAsync()
        {
            return await _db.Bookings
                .Select(b => new BookingDto(b.Id, b.RoomId, b.UserId, b.Date, b.StartTime, b.EndTime, b.Title, b.AttendeesCount, b.Agenda, b.Status, b.CreatedAt))
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingDto>> GetForRoomAsync(int roomId, DateTime date)
        {
            return await _db.Bookings
                .Where(b => b.RoomId == roomId && b.Date == date.Date)
                .Select(b => new BookingDto(b.Id, b.RoomId, b.UserId, b.Date, b.StartTime, b.EndTime, b.Title, b.AttendeesCount, b.Agenda, b.Status, b.CreatedAt))
                .ToListAsync();
        }

        public async Task<BookingDto?> GetByIdAsync(int id)
        {
            var b = await _db.Bookings.FindAsync(id);
            if (b is null) return null;
            return new BookingDto(b.Id, b.RoomId, b.UserId, b.Date, b.StartTime, b.EndTime, b.Title, b.AttendeesCount, b.Agenda, b.Status, b.CreatedAt);
        }

        public async Task<BookingDto> CreateAsync(CreateBookingDto dto, int userId)
        {
            // validate room exists
            var room = await _db.Rooms.FindAsync(dto.RoomId) ?? throw new InvalidOperationException("Room not found");

            // validate times
            if (dto.EndTime <= dto.StartTime) throw new InvalidOperationException("EndTime must be after StartTime");

            // check overlapping bookings for same room and date
            var overlapping = await _db.Bookings.AnyAsync(b => b.RoomId == dto.RoomId && b.Date == dto.Date.Date &&
                !(b.EndTime <= dto.StartTime || b.StartTime >= dto.EndTime) && b.Status != "cancelled");
            if (overlapping) throw new InvalidOperationException("Room already booked for the selected time");

            var booking = new Booking
            {
                RoomId = dto.RoomId,
                UserId = userId,
                Date = dto.Date.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Title = dto.Title,
                Agenda = dto.Agenda,
                AttendeesCount = dto.AttendeesCount,
                Status = "active",
                CreatedAt = DateTime.UtcNow
            };

            _db.Bookings.Add(booking);
            await _db.SaveChangesAsync();

            return new BookingDto(booking.Id, booking.RoomId, booking.UserId, booking.Date, booking.StartTime, booking.EndTime, booking.Title, booking.AttendeesCount, booking.Agenda, booking.Status, booking.CreatedAt);
        }

        public async Task<BookingDto?> UpdateAsync(int id, UpdateBookingDto dto, int userId)
        {
            var b = await _db.Bookings.FindAsync(id);
            if (b is null) return null;

            if (b.UserId != userId) throw new UnauthorizedAccessException();

            // validate times
            if (dto.EndTime <= dto.StartTime) throw new InvalidOperationException("EndTime must be after StartTime");

            // check overlap excluding this booking
            var overlapping = await _db.Bookings.AnyAsync(x => x.Id != id && x.RoomId == dto.RoomId && x.Date == dto.Date.Date &&
                !(x.EndTime <= dto.StartTime || x.StartTime >= dto.EndTime) && x.Status != "cancelled");
            if (overlapping) throw new InvalidOperationException("Room already booked for the selected time");

            b.RoomId = dto.RoomId;
            b.Date = dto.Date.Date;
            b.StartTime = dto.StartTime; 
            b.EndTime = dto.EndTime;
            b.Title = dto.Title;
            b.AttendeesCount = dto.AttendeesCount;
            b.Agenda = dto.Agenda;
            b.Status = dto.Status;

            await _db.SaveChangesAsync();

            return new BookingDto(b.Id, b.RoomId, b.UserId, b.Date, b.StartTime, b.EndTime, b.Title, b.AttendeesCount, b.Agenda, b.Status, b.CreatedAt);
        }

        public async Task<bool> CancelAsync(int id, int userId)
        {
            var b = await _db.Bookings.FindAsync(id);
            if (b is null) return false;

            if (b.UserId != userId) throw new UnauthorizedAccessException();

            b.Status = "cancelled";
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
