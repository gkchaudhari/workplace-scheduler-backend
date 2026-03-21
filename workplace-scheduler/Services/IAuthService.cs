using workplace_scheduler.Dtos;

namespace workplace_scheduler.Services
{
    public interface IAuthService
    {
        Task<UserDto> SignupAsync(SignupDto dto);
        Task<UserDto?> LoginAsync(LoginDto dto);
    }
}
