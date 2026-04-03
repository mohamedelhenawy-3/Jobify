using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny;
using Application.DTOs.SuccefullOp;
using Application.SearchFeature.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Company
{
    public interface ICompanyService
    {
        Task<CompanyOwnerDto> PrivateCompany();
        Task<SuccessResponse> CreateCompany(CreateCompanyDto dto);
        Task<SuccessResponse> UpdateCompany(int companyId, UpdateCompanyDto dto);
        Task<CompanyResponseDto> GetCompanyById(int companyId);
        Task<IEnumerable<CompanyResponseDto>> GetAllCompanies();
        Task<SuccessResponse> DeleteCompany(int companyId);

        // ===== MY COMPANY =====
        Task<CompanyResponseDto> GetMyCompany();

        // ===== JOBS =====
        //Task<IEnumerable<JobPostResponseDto>> GetCompanyJobs(int companyId);
        Task<List<SearchCompanyDto>> SearchCompaniesAsync(GlobalSearchDto dto);

    }
}
