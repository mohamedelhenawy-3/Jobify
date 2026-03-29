using Application.DTOs;
using Application.DTOs.JobApplication;
using Application.DTOs.JobPost;
using Application.Interfaces.JobApplication;
using Application.Interfaces.Skills;
using Domain.Entities.LooksUp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobApplicationController : ControllerBase
    {

        private readonly IJobApplication _IJoobService;

        public JobApplicationController(IJobApplication IJoobService)
        {
            _IJoobService = IJoobService;
        }

        [HttpPost("CreateJobApplication/{id}")]

        public async Task<IActionResult> CreateJob([FromForm] CreateJobApplication createjob, int id)
        {
            var result = await _IJoobService.CreateApplication(createjob, id);
            return Ok(result);

        }

        [HttpGet("MyApplications")]

        public async Task<IActionResult> MyAppications()
        {
            var result = await _IJoobService.GetMyApplications();
            return Ok(result);

        }
        [HttpGet()]

        public async Task<IActionResult> GetAppliedJob()
        {
            var result = await _IJoobService.GetAppliedJobSeeker();
            return Ok(result);

        }
        [HttpGet("AppliedJobDetails/{id}")]
        public async Task<IActionResult> GetAppliedJobDetails(int id )
        {
            var result = await _IJoobService.GetAppliedJobSeekerDeyails(id);
            return Ok(result);

        }
        [HttpPut("AppliedJobDetails/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateAppliedJobStatus dto) // Changed from 'newStatus' to 'dto'
        {
            var result = await _IJoobService.UpdateJobStatus(dto, id);
            return Ok(result);
        }

    }
}
