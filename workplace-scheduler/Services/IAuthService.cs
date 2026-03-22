using workplace_scheduler.Dtos;

namespace workplace_scheduler.Services
{
    public interface IAuthService
    {
        Task<AuthResult> SignupAsync(SignupDto dto);
        Task<AuthResult?> LoginAsync(LoginDto dto);
    }
}
