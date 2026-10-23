using Application.DTOs;
using Application.Interfaces.Stats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {

        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PlatformStatsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<PlatformStatsDto>> Get(CancellationToken ct)
        {
            var stats = await _statsService.GetPlatformStatsAsync(ct);
            return Ok(stats);
        }
    }
}
