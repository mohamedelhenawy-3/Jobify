using Application.DTOs.Employer;
using Application.Interfaces.Company;
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

        private readonly IEmployerService _profileService;

        public EmployerController(
            IEmployerService profileService)
        {
            _profileService = profileService;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            var profile = await _profileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound(new { message = "Company profile not found" });

            return Ok(profile);
        }
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _profileService.GetByEmployerIdAsync(userId);

            if (profile == null)
                return NotFound(new { message = "Company profile not found" });

            return Ok(profile);
        }
        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateEmployerProfile model)
        {
            var result = await _profileService.UpdateAsync(model);
            return Ok(result);
        }
    }
}
