using Application.DTOs.JobPost;
using Application.Interfaces.Company;
using Application.Interfaces.Job;
using Domain.Entities.LooksUp;
using Infrastructure.Implementation.CompanyService;
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

        [HttpPost("CreateJob")]

        public  async Task<IActionResult> CreateJob([FromBody] CreateJob createjob)
        {
            var result = await _jobService.CreateJob(createjob);
            return Ok(result);

        }
        [HttpGet("JobsIndustry/{id:int}")]
        public async Task<IActionResult> IndustryWithJobs(int id )
        {
            var result = await _jobService.JobWithIndustry(id);
            return Ok(result);

        }
        [HttpGet("GetAllIndustriesWithJobCount")]
        public async Task<IActionResult> IndustriesWithJobs()
        {
            var result = await _jobService.Industries();
            return Ok(result);

        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchJobs([FromQuery] JobFilterDto filters,  int pageSize = 10, int page = 1)
        {
            try
            {
                var result = await  _jobService.FilteredJobs(filters, pageSize, page );
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }




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


        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobService.AllJobs();
            return Ok(jobs);
        }

       
        [Authorize(Roles = "Employer")]
        [HttpGet("my-jobs")]
        public async Task<IActionResult> GetMyJobs()
        {

            var employerJobs = await _jobService.AllJobsOfEmployer();
           

            return Ok(employerJobs);
        }

        // DELETE JOB (Owner only)
        [Authorize(Roles = "Employer")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var result = await _jobService.DeleteJob(id);
            return Ok(result);
        }
        // Update JOB (Owner only)
        [Authorize(Roles = "Employer")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateJob([FromBody]UpdateJobDto dto, int id)
        {
            var result = await _jobService.UpdateJob(dto,id);
            return Ok(result);
        }
   

        [HttpGet("recommended")]
        public async Task<IActionResult> GetRecommendedJobs()
        {
            var jobs = await _jobService.GetRecommendedJobs();
            return Ok(jobs);
        }

        [HttpPost("SaveAlert")]
        public async Task<IActionResult> SaveAlert(string keyword)
        {

            var job = await _jobService.SaveAlert(keyword);
            return Ok(job);
        }
        [HttpGet("keyword")]
        public async Task<IActionResult> search(string keyword)
        {

            var job = await _jobService.Search(keyword);
            return Ok(job);
        }
        [HttpGet("OpeningJobs")]
        public async Task<IActionResult> OpeningJibs()
        {
            var result = await _jobService.AvalibleJobs();
            return Ok(result);
        }
    }
}
