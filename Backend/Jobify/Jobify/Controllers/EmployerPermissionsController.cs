using Application.DTOs.Permisions;
using Application.Interfaces.Permisions;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployerPermissionsController : ControllerBase
    {
        private readonly IEmployerPermissionService _permissionService;

        public EmployerPermissionsController(IEmployerPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("{employerId}/permissions")]
        public async Task<IActionResult> GetPermissions(int employerId)
            => Ok(await _permissionService.GetPermissionsAsync(employerId));

        [HttpPost("permissions/grant")]
        public async Task<IActionResult> Grant([FromQuery] int targetEmployerId, [FromQuery] string permission)
            => Ok(await _permissionService.GrantAsync(targetEmployerId, permission));

        [HttpPost("permissions/revoke")]
        public async Task<IActionResult> Revoke([FromQuery] int targetEmployerId, [FromQuery] string permission)
            => Ok(await _permissionService.RevokeAsync(targetEmployerId, permission));
    }

}
