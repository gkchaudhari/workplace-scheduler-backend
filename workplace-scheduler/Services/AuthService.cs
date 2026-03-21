using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using workplace_scheduler.Data;
using workplace_scheduler.Dtos;
using workplace_scheduler.Models;

namespace workplace_scheduler.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<UserDto> SignupAsync(SignupDto dto)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing is not null)
                throw new InvalidOperationException("Email already in use");

            if (!dto.AgreeToTerms)
                throw new InvalidOperationException("You must agree to the terms");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User { Email = dto.Email, PasswordHash = hash, CreatedAt = DateTime.UtcNow, FullName = dto.FullName, AgreeToTerms = dto.AgreeToTerms };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return new UserDto(user.Id, user.Email, user.CreatedAt, user.FullName, user.AgreeToTerms);
        }

        public async Task<UserDto?> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
                return null;

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok) return null;

            return new UserDto(user.Id, user.Email, user.CreatedAt, user.FullName, user.AgreeToTerms);
        }
    }
}
