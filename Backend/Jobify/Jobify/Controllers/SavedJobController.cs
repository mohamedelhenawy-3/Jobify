using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.SavedJobDto;
using Application.Interfaces.SavedJob;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedJobController : ControllerBase
    {


        private readonly ISavedJob _service;
        public SavedJobController(ISavedJob service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<PagedResult<SavedJobDto>>> Mine(
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
            => Ok(await _service.GetSavedJobsAsync(pageNumber, pageSize));

        [HttpGet("{jobId:int}/status")]
        public async Task<ActionResult<SavedJobStatusDto>> Status(int jobId)
            => Ok(new SavedJobStatusDto { IsSaved = await _service.IsJobSavedAsync(jobId) });

        [HttpPut("{jobId:int}")]
        public async Task<IActionResult> Save(int jobId) { await _service.SaveJobAsync(jobId); return NoContent(); }

        [HttpDelete("{jobId:int}")]
        public async Task<IActionResult> Unsave(int jobId) { await _service.UnsaveJobAsync(jobId); return NoContent(); }



    }


}

