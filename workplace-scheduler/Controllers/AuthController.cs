using Microsoft.AspNetCore.Mvc;
using workplace_scheduler.Dtos;
using workplace_scheduler.Services;

namespace workplace_scheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            try
            {
                var user = await _auth.SignupAsync(dto);
                return CreatedAtAction(null, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _auth.LoginAsync(dto);
            if (user is null) return Unauthorized();
            return Ok(user);
        }
    }
}
