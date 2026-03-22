using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using workplace_scheduler.Data;
using workplace_scheduler.Dtos;
using workplace_scheduler.Models;

namespace workplace_scheduler.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<AuthResult> SignupAsync(SignupDto dto)
        {
            var existing = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existing is not null)
                throw new InvalidOperationException("Email already in use");

            if (!dto.AgreeToTerms)
                throw new InvalidOperationException("You must agree to the terms");

            var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var user = new User { Email = dto.Email, PasswordHash = hash, CreatedAt = DateTime.UtcNow, FullName = dto.FullName, AgreeToTerms = dto.AgreeToTerms, Role = "Employee" };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            // generate token
            var token = GenerateToken(user);
            return token;
        }

        public async Task<AuthResult?> LoginAsync(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null)
                return null;

            var ok = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!ok) return null;

            var token = GenerateToken(user);
            return token;
        }

        private AuthResult GenerateToken(User user)
        {
            var jwt = _config.GetSection("Jwt");
            var key = jwt.GetValue<string>("Key");
            var issuer = jwt.GetValue<string>("Issuer");
            var audience = jwt.GetValue<string>("Audience");
            var expires = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpiresInMinutes"));

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("fullName", user.FullName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return new AuthResult(tokenStr, expires);
        }
    }
}
