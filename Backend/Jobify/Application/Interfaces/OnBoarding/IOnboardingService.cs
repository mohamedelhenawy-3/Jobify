using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.OnBoarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.OnBoarding
{
    public interface IOnboardingService
    {
        Task<CreateCompanyResponseDto> CreateCompanyAndLinkEmployerAsync(
          CreateCompanyOnboardingDto dto,
                   CancellationToken cancellationToken);
        Task<List<CompanySearchDto>> SearchCompaniesAsync(string query);
        Task RequestToJoinCompanyAsync(int companyId);
        Task ProcessJoinRequestAsync(ProcessJoinRequestDto dto);


        Task<PagedResult<JoinRequestEmployers>> GetJoinRequestsAsync(JoinRequestFilterDto f);
        Task<EmployerOnboardingStatusDto> GetEmployerOnboardingStatusAsync(
            CancellationToken cancellationToken = default);
    }
}
