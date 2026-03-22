using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workplace_scheduler.Dtos;
using workplace_scheduler.Services;

namespace workplace_scheduler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _booking;

        public BookingsController(IBookingService booking)
        {
            _booking = booking;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _booking.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("room/{roomId}")]
        public async Task<IActionResult> GetForRoom(int roomId, [FromQuery] DateTime date)
        {
            var list = await _booking.GetForRoomAsync(roomId, date);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var b = await _booking.GetByIdAsync(id);
            if (b is null) return NotFound();
            return Ok(b);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBookingDto dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null) return Unauthorized();
            var userId = int.Parse(userIdClaim);

            try
            {
                var created = await _booking.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBookingDto dto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null) return Unauthorized();
            var userId = int.Parse(userIdClaim);

            try
            {
                var updated = await _booking.UpdateAsync(id, dto, userId);
                if (updated is null) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null) return Unauthorized();
            var userId = int.Parse(userIdClaim);

            try
            {
                var ok = await _booking.CancelAsync(id, userId);
                if (!ok) return NotFound();
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}
