using Application.DTOs.Comapny;
using Application.Interfaces.Company;
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

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> CreateCompany([FromForm] CreateCompanyDto dto)
        {
            var result = await _companyService.CreateCompany(dto);
            return Ok(result);
        }

        [HttpPut("{companyId}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromForm] UpdateCompanyDto dto)
        {
            var result = await _companyService.UpdateCompany(companyId, dto);
            return Ok(result);
        }

        [HttpDelete("{companyId}")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> DeleteCompany(int companyId)
        {
            var result = await _companyService.DeleteCompany(companyId);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Employer")]
        public async Task<IActionResult> GetMyCompany()
        {
            var result = await _companyService.GetMyCompany();
            return Ok(result);
        }

        [HttpGet("{companyId}")]

        public async Task<IActionResult> GetCompanyById(int companyId)
        {
            var result = await _companyService.GetCompanyById(companyId);
            return Ok(result);
        }

        [HttpGet]
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
 

    }
}
