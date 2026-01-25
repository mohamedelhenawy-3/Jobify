using Application.DTOs.JobSeeker;
using Application.Interfaces.JobSeeker;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobSeekerController : ControllerBase
    {
        private readonly IJobSeekerService _jobSeekerService;

        public JobSeekerController(IJobSeekerService jobSeekerService)
        {
            _jobSeekerService = jobSeekerService;
        }



        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _jobSeekerService.GetMyProfile();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobSeekerProfile(int id)
        {
            var result = await _jobSeekerService.GetJobSeekerProfile(id);
            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateMyProfile dto)
        {
            var result = await _jobSeekerService.UpdateMyProfile(dto);
            return Ok(result);
        }
        [HttpPost("UploadMyCv")]
        [Authorize()]
        public async Task<IActionResult> UploadMyCv([FromForm] UploadMyResume dto)
        {
            var result = await _jobSeekerService.UploadCv(dto);
            return Ok(result);
        }

        [HttpGet("my-cv")]
        public async Task<ActionResult<ResumeDto>> ViewMyCv()
        {
            var result = await _jobSeekerService.ViewMyCv();
            return Ok(result);
        }
    }
}
