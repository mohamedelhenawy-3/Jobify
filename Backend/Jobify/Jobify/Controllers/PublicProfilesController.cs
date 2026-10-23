using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.Employer;
using Application.DTOs.JobSeeker.JobSeekers;
using Application.Interfaces.PublicProfile;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicProfilesController : ControllerBase
    {
        private readonly IPublicProfileService _service;

        public PublicProfilesController(IPublicProfileService service)
        {
            _service = service;
        }
        [HttpGet("PublicProfile/{id}")]
        public async Task<ActionResult<PublicEmployerProfileDto>> GetPublicProfilr(string id)
        {
            var p = await _service.GetUserProfileAsync(id);
            return p is null ? NotFound() : Ok(p);
        }

        [HttpGet("employers/{id}")]
        public async Task<ActionResult<PublicEmployerProfileDto>> GetEmployer(string id)
        {
            var p = await _service.GetEmployerByIdAsync(id);
            return p is null ? NotFound() : Ok(p);
        }

        [HttpGet("jobseekers/{id}")]
        public async Task<ActionResult<PublicJobSeekerProfileDto>> GetJobSeeker(string id)
        {
            var p = await _service.GetJobSeekerByIdAsync(id);
            return p is null ? NotFound() : Ok(p);
        }
        [HttpGet("companies/{id:int}")]
        public async Task<ActionResult<PublicCompanyProfileDto>> GetCompany(int id, [FromQuery] int maxJobs = 10)
        {
            var company = await _service.GetCompanyProfileAsync(id, maxJobs);
            return company is null
                ? NotFound(new { message = "Company not found or not public" })
                : Ok(company);
        }
    }
}
