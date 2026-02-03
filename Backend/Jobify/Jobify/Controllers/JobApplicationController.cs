using Application.DTOs;
using Application.DTOs.JobPost;
using Application.Interfaces.JobApplication;
using Application.Interfaces.Skills;
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

        public async Task<IActionResult> CreateJob([FromBody] CreateJobApplication createjob,int id)
        {
            var result = await _IJoobService.CreateApplication(createjob,id);
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

    }
}
