using Application.DTOs.Skills;
using Application.Interfaces.User;
using Infrastructure.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUser _userservice;
        public UserController(IUser userservice)
        {
            _userservice = userservice;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getUser (string id)
        {
            var result = await _userservice.GetUserProfileAsync(id);
            return Ok(result);
        }
        [HttpGet("me/summary")]
        public async Task<IActionResult> GetMySummary()
        {
            var summary = await _userservice.GetMySummaryAsync();
            return summary == null ? NotFound() : Ok(summary);
        }


    }
}
