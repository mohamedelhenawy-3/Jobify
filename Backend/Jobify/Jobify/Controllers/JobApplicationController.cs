using Application.DTOs;
using Application.DTOs.JobApplication;
using Application.DTOs.JobApplication.JobApplication;
using Application.DTOs.JobPost;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.JobApplication;
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
  

        [HttpGet("my-applications")]
        public async Task<ActionResult> GetMyApplications([FromQuery] ApplicationFilterDto filter)
        {

            var result = await _IJoobService.GetMyApplications(filter);
            return Ok(result);
        }
        [HttpGet("appliedseekers")]
        public async Task<ActionResult> AppliedSeekers([FromQuery] ApplicationFilterDto filter)
        {

            var result = await _IJoobService.appliedapplications(filter);
            return Ok(result);
        }

        [HttpGet("recent-applicants")]
        public async Task<IActionResult> GetRecentApplicants([FromQuery] int take = 5) => Ok(await _IJoobService.GetRecentApplicantsAsync(take));
        [HttpGet("pipeline")]
        public async Task<IActionResult> GetPipeline() => Ok(await _IJoobService.GetPipelineAsync());

        [HttpGet("seeker")]
        public async Task<IActionResult> GetSeekerApplications(
            [FromQuery] JobApplicationFilterDto filter)
        {
            try
            {
                var result = await _IJoobService.GetApplicationsForSeekerAsync(filter);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }

        [HttpGet("employer")]
        public async Task<IActionResult> GetEmployerApplications([FromQuery] JobApplicationFilterDto filter)
        {
            var result = await _IJoobService.GetEmployerApplicationsAsync(filter);
            return Ok(result);
        }
        [HttpPut("{applicationId:int}/status")]
        public async Task<ActionResult<SuccessResponse>> UpdateStatus(
    int applicationId,
    [FromBody] UpdateAppliedJobStatus dto)
        {
            try
            {
                var result = await _IJoobService.UpdateJobStatus(dto, applicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/employer/applications/5
        [HttpGet("{applicationId:int}")]
        public async Task<ActionResult<ApplicationDetailsDto>> GetDetails(int applicationId)
        {
            try
            {
                return Ok(await _IJoobService.GetApplicationDetails(applicationId));
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
