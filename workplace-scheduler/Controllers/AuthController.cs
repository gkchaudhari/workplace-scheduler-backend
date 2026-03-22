using Microsoft.AspNetCore.Authorization;
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

        //authentication endpoints are usually public, so we allow anonymous access
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> Signup([FromBody] SignupDto dto)
        {
            try
            {
                var result = await _auth.SignupAsync(dto);
                return Created(string.Empty, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _auth.LoginAsync(dto);
            if (result is null) return Unauthorized();
            return Ok(result);
        }
    }
}
