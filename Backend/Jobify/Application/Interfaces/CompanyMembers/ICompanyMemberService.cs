using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.CompanyMembers;
using Application.DTOs.JobPost.JobDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.CompanyMembers
{
    public interface ICompanyMemberService
    {
        Task<CompanyProfileDto> GetMyCompanyAsync();
        Task<CompanyProfileDto> UpdateMyCompanyAsync(UpdateCompanyDto dto);

        Task<CompanyMembersPreviewDto> GetMembersAsync(int take = 5);
        Task<MyTeamRoleDto> GetMyRoleAsync();
        Task<SuccessResponseDto> UpdatePermissionsAsync(int memberId, UpdatePermissionsDto dto);


        Task<PagedResult<TeamMemberDto>> GetMembers(TeamFilterDto f);
         Task SetActive(int memberId, SetActiveDto dto);
        Task<SuccessResponseDto> ChangeRoleAsync(int memberId, ChangeMemberRoleDto dto);
        Task<SuccessResponseDto> RemoveMemberAsync(int targetEmployerId);
        Task<SuccessResponseDto> LeaveCompanyAsync();
        Task<SuccessResponseDto> TransferOwnershipAsync(int newOwnerEmployerId);
    }
}
