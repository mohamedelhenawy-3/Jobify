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

        public SavedJobController(ISavedJob service)
        {
            _service = service;
        }

        [HttpPost("{jobId}")]
        public async Task<IActionResult> SaveJob(int jobId)
        {
           
            await _service.SaveJobAsync(jobId);
            return Ok();
        }

        [HttpDelete("{jobId}")]
        public async Task<IActionResult> UnsaveJob(int jobId)
        {
            await _service.UnsaveJobAsync(jobId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSavedJobs()
        {
            var jobs = await _service.GetSavedJobsAsync();
            return Ok(jobs);
        }

        [HttpGet("check/{jobId}")]
        public async Task<IActionResult> IsSaved(int jobId)
        {
            return Ok(await _service.IsJobSavedAsync(jobId));
        }
    }
}
