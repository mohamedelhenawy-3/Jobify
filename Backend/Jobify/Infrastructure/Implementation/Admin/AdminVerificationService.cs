using Application.DTOs.Admin;
using Application.Interfaces.Admin;
using Domain.Entities.LooksUp;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Admin
{
    public class AdminVerificationService :IAdminVerificationService
    {
        private readonly AppDBContext _context;

        public AdminVerificationService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<CompanyPendingTriageDto> GetCompanyDetailsForReviewAsync(int companyId)
        {
            var company = await _context.Companies
                .Include(c => c.CreatedByEmployer)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
            {
                throw new KeyNotFoundException($"Company with ID {companyId} was not found.");
            }

            return new CompanyPendingTriageDto
            {
                CompanyId = company.Id,
                CompanyName = company.CompanyName,
                Industry = company.Industry ?? string.Empty,
                SubmittedByEmployerName = company.CreatedByEmployer?.User?.FullName ?? string.Empty,
                SubmittedByEmail = company.CreatedByEmployer?.User?.Email ?? string.Empty,
                VerificationSubmittedAt = company.VerificationSubmittedAt
            };
        }

        public async Task<List<CompanyPendingTriageDto>> GetPendingQueueAsync()
        {
            return await _context.Companies
                .Where(c => c.VerificationStatus == CompanyVerificationStatus.Pending)
                .OrderBy(c => c.VerificationSubmittedAt)
                .Select(c => new CompanyPendingTriageDto
                {
                    CompanyId = c.Id,
                    CompanyName = c.CompanyName,
                    Industry = c.Industry ?? string.Empty,
                    SubmittedByEmployerName = c.CreatedByEmployer.User.FullName ?? string.Empty,
                    SubmittedByEmail = c.CreatedByEmployer.User.Email ?? string.Empty,
                    VerificationSubmittedAt = c.VerificationSubmittedAt
                })
                .ToListAsync();
        }

        public async Task ProcessDecisionAsync(int companyId, VerifyCompanyDecisionDto dto)
        {
            var company = await _context.Companies.FindAsync(companyId);
            if (company == null) throw new Exception("Company not found.");

            if (dto.IsApproved)
            {
                company.VerificationStatus = CompanyVerificationStatus.Approved;
                company.IsVerified = true;
                company.RejectionReason = null;
            }
            else
            {
                company.VerificationStatus = CompanyVerificationStatus.Rejected;
                company.IsVerified = false;
                company.RejectionReason = dto.RejectionReason;
            }

            await _context.SaveChangesAsync();
        }
    }
}
