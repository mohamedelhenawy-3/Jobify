using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny;
using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.Comapny.RefactorCompany.RecordDto;
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

        Task<IReadOnlyList<TopHiringCompanyDto>> GetTopHiringCompaniesAsync(int count = 8, CancellationToken ct = default);

        Task<PublicCompanyDto> GetByIdAsync(int id);
        Task<CompanyOwnerDto> PrivateCompany();
        Task<CompanyResponseDto> GetCompanyById(int companyId);
        Task<IEnumerable<CompanyResponseDto>> GetAllCompanies();
        // ===== JOBS =====
        Task<List<SearchCompanyDto>> SearchCompaniesAsync(GlobalSearchDto dto);

        Task<HiringSnapshotDto?> GetHiringSnapshotAsync();
    }
}
