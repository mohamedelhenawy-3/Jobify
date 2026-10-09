using Application.DTOs.JobPost;
using Application.DTOs.JobPost.JobDTO;
using Application.Interfaces.Company;
using Application.Interfaces.Job;
using Domain.Entities.LooksUp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Employer")]

    public class JobController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobController(
            IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("EmpJobs")]
        public async Task<IActionResult> GetMyJobs()
        {
            return Ok(await _jobService.GetMyJobsAsync());
        }
        [HttpGet("EmployerJobs")]
        public async Task<IActionResult> GetMyJobs([FromQuery] EmployerJobsFilterDto filter)
        {
            var result = await _jobService.GetEmployerJobsAsync(filter);

            if (result == null)
                return Forbid();

            return Ok(result);
        }

        [HttpPost("CreateJob")]
        public  async Task<IActionResult> CreateJob([FromBody] CreateJobPostDto createjob)
        {
            var result = await _jobService.CreateJob(createjob);
            return Ok(result);

        }
        [HttpGet("{id}/detailsJobSeeker-Job")]
        [Authorize] 
        public async Task<IActionResult> GetJobDetailsForJobseeker(int id)
        {
            var result = await _jobService.GetJobDetailsForJobseeker(id);
            return Ok(result);
        }


        [HttpGet("Jobs")]
        [AllowAnonymous]  
        public async Task<IActionResult> GetJobs([FromQuery] JobsFilterDto filter)
     => Ok(await _jobService.GetActiveJobsAsync(filter));


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var job = await _jobService.GetJobById(id);
            if (job == null)
                return NotFound("Job not found");

            return Ok(job);
        }
        [HttpGet("myjob/{id:int}")]
        public async Task<IActionResult> GetJob(int id)
        {
            var job = await _jobService.GetJob(id);
            if (job == null)
                return NotFound("Job not found");

            return Ok(job);
        }


        [HttpGet("my-jobs")]
        public async Task<IActionResult> GetMyJobss()
        {
            var jobs = await _jobService.GetEmployerJobsAsync();
            return jobs == null ? Forbid() : Ok(jobs);
        }




        /// <summary>
        /// ///////////////////////////////////////////////////////////////////Finish until here
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        // DELETE JOB (Owner only)
        [Authorize(Roles = "Employer")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var result = await _jobService.DeleteJob(id);
            return Ok(result);
        }
        // Update JOB (Owner only)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateJob([FromBody] UpdateJobDto dto, int id)
        {
            try
            {
                var result = await _jobService.UpdateJob(dto,id);
                return Ok(new { message = "Job post updated successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new { status = 403, message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { status = 404, message = ex.Message });
            }
        }



        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedJobs()
        {
            var jobs = await _jobService.GetRecommendedJobs();
            return Ok(jobs);
        }

        [HttpPost("SaveAlert")]
        public async Task<IActionResult> SaveAlert([FromBody]string keyword)
        {

            var job = await _jobService.SaveAlert(keyword);
            return Ok(job);
        }
        [HttpPost("filter")]
        public async Task<IActionResult> FilteredJobs([FromBody] JobFilterDto dto)
        {
            var result = await _jobService.Search(dto);
            return Ok(result);
        }
     
    }
}
