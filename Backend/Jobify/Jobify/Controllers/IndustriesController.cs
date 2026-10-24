using Application.DTOs.Industries;
using Application.Interfaces.Industry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustriesController : ControllerBase
    {

        private readonly IIndustryService _service;

        public IndustriesController(IIndustryService service) => _service = service;

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<IndustryDto>>> GetAll(
            [FromQuery] bool includeInactive = false, CancellationToken ct = default)
        {
            if (includeInactive && !User.IsInRole("Admin"))
                includeInactive = false;

            return Ok(await _service.GetAllAsync(includeInactive, ct));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public Task<IActionResult> GetById(int id, CancellationToken ct) =>
            Run(async () => Ok(await _service.GetByIdAsync(id, ct)));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Create(CreateIndustryDto dto, CancellationToken ct) =>
            Run(async () =>
            {
                var created = await _service.CreateAsync(dto, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            });

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Update(int id, UpdateIndustryDto dto, CancellationToken ct) =>
            Run(async () =>
            {
                await _service.UpdateAsync(id, dto, ct);
                return NoContent();
            });

        // DELETE api/industries/5
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public Task<IActionResult> Delete(int id, CancellationToken ct) =>
            Run(async () =>
            {
                await _service.DeleteAsync(id, ct);
                return NoContent();
            });


        [HttpGet("popular")]
        public async Task<ActionResult<List<PopularIndustryDto>>> GetPopular([FromQuery] int take = 6)
        {
            take = Math.Clamp(take, 1, 20);
            return Ok(await _service.GetPopularAsync(take));
        }
        private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
        {
            try { return await action(); }
            catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
            catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }
    }
}
