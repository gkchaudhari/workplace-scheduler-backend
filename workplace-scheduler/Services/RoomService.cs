using Microsoft.EntityFrameworkCore;
using workplace_scheduler.Data;
using workplace_scheduler.Dtos;
using workplace_scheduler.Models;

namespace workplace_scheduler.Services
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _db;

        public RoomService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<RoomDto>> GetAllAsync()
        {
            return await _db.Rooms
                .Select(r => new RoomDto(r.Id, r.Name, r.Location, r.Capacity, r.IsActive, r.CreatedAt))
                .ToListAsync();
        }

        public async Task<RoomDto?> GetByIdAsync(int id)
        {
            var r = await _db.Rooms.FindAsync(id);
            if (r is null) return null;
            return new RoomDto(r.Id, r.Name, r.Location, r.Capacity, r.IsActive, r.CreatedAt);
        }

        public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
        {
            var room = new Room { Name = dto.Name, Location = dto.Location, Capacity = dto.Capacity, CreatedAt = DateTime.UtcNow };
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
            return new RoomDto(room.Id, room.Name, room.Location, room.Capacity, room.IsActive, room.CreatedAt);
        }

        public async Task<RoomDto?> UpdateAsync(int id, UpdateRoomDto dto)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room is null) return null;
            room.Name = dto.Name;
            room.Location = dto.Location;
            room.Capacity = dto.Capacity;
            room.IsActive = dto.IsActive;
            await _db.SaveChangesAsync();
            return new RoomDto(room.Id, room.Name, room.Location, room.Capacity, room.IsActive, room.CreatedAt);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var room = await _db.Rooms.FindAsync(id);
            if (room is null) return false;
            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
