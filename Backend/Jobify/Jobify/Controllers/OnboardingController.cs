using Application.DTOs.Comapny;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.OnBoarding;
using Application.Interfaces.Company;
using Application.Interfaces.OnBoarding;
using Infrastructure.Implementation.OnBoardingService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(
            IOnboardingService onboardingService
           )
        {
            _onboardingService = onboardingService;
        }


        [HttpPost]
        public async Task<IActionResult> CreateCompany(
           CreateCompanyOnboardingDto dto,
           CancellationToken cancellationToken)
        {
            try
            {
                var result = await _onboardingService.CreateCompanyAndLinkEmployerAsync(dto, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("search-companies")]
        public async Task<IActionResult> SearchCompanies([FromQuery] string query)
        {
            var results = await _onboardingService.SearchCompaniesAsync(query);
            return Ok(results);
        }

        [HttpPost("join-company")]
        public async Task<IActionResult> JoinCompany([FromBody] JoinCompanyRequestDto dto)
        {
            await _onboardingService.RequestToJoinCompanyAsync( dto.CompanyId);
            return Ok(new { Message = "Join request submitted successfully. Awaiting owner approval." });
        }

        //employer owner for company
        [HttpPost("process-join-request")]
        public async Task<IActionResult> ProcessJoinRequest([FromBody] ProcessJoinRequestDto dto)
        {
            await _onboardingService.ProcessJoinRequestAsync(dto);
            return Ok(new { Message = "Join request status updated." });
        }

        [HttpGet("status")]
        public async Task<ActionResult<EmployerOnboardingStatusDto>> GetStatus(
            CancellationToken cancellationToken)
        {
            var status = await _onboardingService.GetEmployerOnboardingStatusAsync(cancellationToken);
            return Ok(status);
        }

        [HttpGet("join-requests")]
        public async Task<ActionResult<PagedResult<JoinRequestEmployers>>> JoinRequests([FromQuery] JoinRequestFilterDto f) => Ok(await _onboardingService.GetJoinRequestsAsync(f));

    }
}
