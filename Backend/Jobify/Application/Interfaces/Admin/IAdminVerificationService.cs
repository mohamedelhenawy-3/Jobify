using Application.DTOs.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Admin
{
    public interface IAdminVerificationService
    {
        Task<List<CompanyPendingTriageDto>> GetPendingQueueAsync();
        Task ProcessDecisionAsync(int companyId, VerifyCompanyDecisionDto dto);
        Task<CompanyPendingTriageDto> GetCompanyDetailsForReviewAsync(int companyId);
    }
}
