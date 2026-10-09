using Application.DTOs.Admin;
using Application.Interfaces.Admin;
using Infrastructure.Implementation.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/admin/companies")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminCompanyController : ControllerBase
    {
        private readonly IAdminVerificationService _adminVerificationService;

        public AdminCompanyController(IAdminVerificationService adminVerificationService)
        {
            _adminVerificationService = adminVerificationService;
        }

        [HttpGet("pending")]
        [ProducesResponseType(typeof(List<CompanyPendingTriageDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingQueue()
        {
            var pendingCompanies = await _adminVerificationService.GetPendingQueueAsync();
            return Ok(pendingCompanies);
        }

        [HttpGet("{companyId:int}")]
        [ProducesResponseType(typeof(CompanyPendingTriageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCompanyForReview(int companyId)
        {
            try
            {
                var companyDetails = await _adminVerificationService.GetCompanyDetailsForReviewAsync(companyId);
                return Ok(companyDetails);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

     
        [HttpPost("{companyId:int}/verify")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyCompany(int companyId, [FromBody] VerifyCompanyDecisionDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _adminVerificationService.ProcessDecisionAsync(companyId, dto);

                var statusMessage = dto.IsApproved ? "approved" : "rejected";
                return Ok(new
                {
                    CompanyId = companyId,
                    IsApproved = dto.IsApproved,
                    Message = $"Company ID {companyId} has been successfully {statusMessage}."
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
