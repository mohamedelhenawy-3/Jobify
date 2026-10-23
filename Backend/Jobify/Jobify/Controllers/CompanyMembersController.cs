using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.CompanyMembers;
using Application.DTOs.JobPost.JobDTO;
using Application.Interfaces.CompanyMembers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyMembersController : ControllerBase
    {
        private readonly ICompanyMemberService _memberService;

        public CompanyMembersController(ICompanyMemberService memberService)
            => _memberService = memberService;

        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCompany([FromForm] UpdateCompanyDto dto)
        {
            try
            {
                var response = await _memberService.UpdateMyCompanyAsync(dto);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var result = await _memberService.GetMyCompanyAsync();

            if (result == null)
                return NotFound("You do not belong to a company.");

            return Ok(result);
        }

        [HttpGet("members")]
        public async Task<IActionResult> GetMembersPreview([FromQuery] int take = 5)
        {
            var result = await _memberService.GetMembersAsync(take);

            if (result == null)
                return NotFound("You do not belong to a company.");

            return Ok(result);
        }



        [HttpPut("members/{id:int}/active")]
        public async Task<IActionResult> Active(int id, SetActiveDto dto)
        {
            await _memberService
                .SetActive(id, dto);
            return NoContent();
        }

        [HttpPut("members/{id:int}/role")]
        public async Task<IActionResult> ChangeRole(int id, ChangeMemberRoleDto dto)
        {
            await _memberService.ChangeRoleAsync(id, dto);
            return NoContent();
        }
        [HttpDelete("members/{memberId:int}")]
        public async Task<IActionResult> RemoveMember(int memberId, [FromBody] RemoveMemberDto? dto = null)
        {
            var result = await _memberService.RemoveMemberAsync(memberId, dto);
            return Ok(result);
        }

        [HttpPost("members/leave")]
        public async Task<IActionResult> LeaveCompany([FromBody] RemoveMemberDto? dto = null)
        {
            var result = await _memberService.LeaveCompanyAsync(dto);
            return Ok(result);
        }


        [HttpPost("transfer-ownership/{newOwnerEmployerId}")]
        public async Task<IActionResult> Transfer(int newOwnerEmployerId)
            => Ok(await _memberService.TransferOwnershipAsync(newOwnerEmployerId));

        [HttpGet("membersfilters")]
        public async Task<ActionResult<PagedResult<TeamMemberDto>>> Members([FromQuery] TeamFilterDto f)
    => Ok(await _memberService.GetMembers(f));

        [HttpGet("me")]
        public async Task<ActionResult<MyTeamRoleDto>> Me()
    => Ok(await _memberService.GetMyRoleAsync());


        [HttpPut("members/{id:int}/permissions")]
        public async Task<ActionResult<SuccessResponseDto>> Permissions(int id, UpdatePermissionsDto dto)
            => Ok(await _memberService.UpdatePermissionsAsync(id, dto));



        [HttpGet("activity")]
        public async Task<ActionResult<PagedResult<ActivityDto>>> Activity([FromQuery] ActivityFilterDto f)
    => Ok(await _memberService.GetActivityAsync(f));



        /// <summary>Activities where I am the actor or the target.</summary>
        [HttpGet("activity/me")]
        public async Task<IActionResult> GetMyActivity([FromQuery] ActivityFilterDto filter) =>
            Ok(await _memberService.GetMyActivityAsync(filter));

        [HttpGet("companyactivity")]
        public async Task<IActionResult> GetCompanyActivity([FromQuery] ActivityFilterDto filter) =>
            Ok(await _memberService.GetCompanyActivityAsync(filter));
    }


}

