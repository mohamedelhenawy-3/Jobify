using Application.DTOs.Comapny;
using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.Comapny.RefactorCompany.RecordDto;
using Application.Interfaces.Company;
using Application.Interfaces.PublicCompany;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IPublicCompanyService _publiccompanyservice;

        public CompanyController(ICompanyService companyService, IPublicCompanyService service)
        {
            _companyService = companyService;
            _publiccompanyservice = service;
        }


        // GET api/companies?q=tech&industry=IT&location=Cairo
        [HttpGet("search")]
        public async Task<IActionResult> Search(string? q, string? industry, string? location)
        {
            return Ok(await _publiccompanyservice.SearchAsync(q, industry, location));
        }
        // GET api/companies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _publiccompanyservice.GetByIdAsync(id);

            if (result == null)
                return NotFound("Company not found.");

            return Ok(result);
        }

        [HttpGet("hiring-snapshot")]
        public async Task<IActionResult> GetHiringSnapshot()
        {
            var result = await _companyService.GetHiringSnapshotAsync();

            if (result == null)
                return NotFound("You do not belong to a company.");

            return Ok(result);
        }


        [HttpGet("allcompanies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCompanies()
        {
            var result = await _companyService.GetAllCompanies();
            return Ok(result);
        }
        [HttpGet("privatrcompany")]
        
        public async Task<IActionResult> PrivateCompany()
        {
            var result = await _companyService.PrivateCompany();
            return Ok(result);
        }
        [HttpGet("top-hiring")]
        [ProducesResponseType(typeof(IReadOnlyList<TopHiringCompanyDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<TopHiringCompanyDto>>> GetTopHiring(
              [FromQuery] int count = 8, CancellationToken ct = default)
        {
            var companies = await _companyService.GetTopHiringCompaniesAsync(count, ct);
            return Ok(companies);
        }

    }
}
