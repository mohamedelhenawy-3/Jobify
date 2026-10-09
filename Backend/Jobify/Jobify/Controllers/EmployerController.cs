using Application.DTOs.Employer;
using Application.Interfaces.Company;
using Application.Interfaces.Employer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Employer")]

    public class EmployerController : ControllerBase
    {

        private readonly IEmployer _profileService;

        public EmployerController(
            IEmployer profileService)
        {
            _profileService = profileService;
        }
    
        //[HttpGet]
        //public async Task<IActionResult> GetProfile()
        //{
        //    var profile = await _profileService.MyProfile();

        //    if (profile == null)
        //        return NotFound(new { message = "Company profile not found" });

        //    return Ok(profile);
        //}
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var profile = await _profileService.GetOwnerProfileAsync();
            return profile == null ? Forbid() : Ok(profile);
        }

        [HttpPut("profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateEmployerProfile model)
        {
            var result = await _profileService.UpdateAsync(model);
            return Ok(result);
        }
    }
}
