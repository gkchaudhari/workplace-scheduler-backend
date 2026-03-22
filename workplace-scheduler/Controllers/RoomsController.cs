using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using workplace_scheduler.Dtos;
using workplace_scheduler.Services;

namespace workplace_scheduler.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _rooms;
        public RoomsController(IRoomService rooms) => _rooms = rooms;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _rooms.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var room = await _rooms.GetByIdAsync(id);
            if (room is null) return NotFound();
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateRoomDto dto)
        {
            var room = await _rooms.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = room.Id }, room);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRoomDto dto)
        {
            var room = await _rooms.UpdateAsync(id, dto);
            if (room is null) return NotFound();
            return Ok(room);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _rooms.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
