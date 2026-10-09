using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.OnBoarding;
using Application.Interfaces.OnBoarding;
using Application.Interfaces.Permisions;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Domain.Entities.LooksUp.EmployerPermisions;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.OnBoardingService
{
    public class OnboardingService : IOnboardingService
    {
        private readonly AppDBContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly IEmployerPermissionService _permissionService;

        public OnboardingService(
            AppDBContext context,
            ICurrentUser currentUser,IEmployerPermissionService permision)
        {
            _context = context;
            _permissionService = permision;
            _currentUser = currentUser;
        }
        public async Task<CreateCompanyResponseDto>
               CreateCompanyAndLinkEmployerAsync(
                   CreateCompanyOnboardingDto dto,
                   CancellationToken cancellationToken)
        {
            var employer = await _context.EmployerProfiles
      .FirstOrDefaultAsync(
          e => e.UserId == _currentUser.UserId,
          cancellationToken);

            if (employer == null)
                throw new Exception("Employer profile not found.");

            var company = new Company
            {
                CompanyName = dto.CompanyName,
                CompanyWebsite = dto.CompanyWebsite,
                Description = dto.Description,
                Location = dto.Location,
                Industry = dto.Industry,
                CompanySize = dto.CompanySize,
                Founded = dto.Founded,

                CreatedByEmployerId = employer.Id,

                IsVerified = false,
                VerificationStatus = CompanyVerificationStatus.Pending,
                VerificationSubmittedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);

            await _context.SaveChangesAsync(cancellationToken);

            employer.CompanyId = company.Id;
            employer.Role = EmployerCompanyRole.Owner;
            employer.Permissions = EmployerCompanyRole.Owner.DefaultPermissions();
            employer.JoinedCompanyAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return new CreateCompanyResponseDto
            {
                CompanyId = company.Id,
                Message = "Company submitted for verification."
            };
        }

        public async Task<EmployerOnboardingStatusDto> GetEmployerOnboardingStatusAsync(CancellationToken cancellationToken = default)
        {
            var employer = await _context.EmployerProfiles
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId, cancellationToken);

            if (employer == null)
                throw new Exception("Employer profile not found.");

            // Case 1: Employer is linked to a company
            if (employer.CompanyId != null && employer.Company != null)
            {
                return new EmployerOnboardingStatusDto
                {
                    HasCompany = true,
                    CompanyId = employer.CompanyId,
                    CompanyName = employer.Company.CompanyName,
                    IsVerified = employer.Company.IsVerified,
                    VerificationStatus = employer.Company.VerificationStatus.ToString(),
                    HasPendingJoinRequest = false
                };
            }

            // Case 2: Employer has requested to join an existing company
            var pendingJoinRequest = await _context.CompanyJoinRequests
                .FirstOrDefaultAsync(
                    r => r.EmployerProfileId == employer.Id && r.Status == JoinRequestStatus.Pending,
                    cancellationToken);

            return new EmployerOnboardingStatusDto
            {
                HasCompany = false,
                HasPendingJoinRequest = pendingJoinRequest != null,
                PendingCompanyId = pendingJoinRequest?.CompanyId,
                VerificationStatus = pendingJoinRequest != null ? "Pending" : "None"
            };
        }

        public async Task<PagedResult<JoinRequestEmployers>> GetJoinRequestsAsync(JoinRequestFilterDto f)
        {
            var actor = await _permissionService.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.ApproveJoinRequests)
              ?? throw new UnauthorizedAccessException("You can't review join requests.");

            f.PageNumber = Math.Max(f.PageNumber, 1);
            f.PageSize = Math.Clamp(f.PageSize, 1, 50);

            var q = _context.CompanyJoinRequests
                    .AsNoTracking()
                    .Where(r => r.CompanyId == actor.CompanyId);

            if (f.Status.HasValue) q = q.Where(r => r.Status == f.Status);
            var total = await q.CountAsync();

            var items = await q
                .OrderByDescending(r => r.RequestedAt).ThenBy(r => r.Id)
                .Skip((f.PageNumber - 1) * f.PageSize)
                .Take(f.PageSize)
                .Select(r => new JoinRequestEmployers
                {
                    Id = r.Id,
                    logo=r.EmployerProfile.ProfileImage.ImageUrl,
                    FullName = r.EmployerProfile.User.FullName,
                    Email = r.EmployerProfile.User.Email,
                    Status = r.Status,
                    CreatedAt = r.RequestedAt,
                    RespondedAt = r.RespondedAt
                })
                .ToListAsync();

            return new PagedResult<JoinRequestEmployers>
            {
                Items = items,
                TotalCount = total,
                PageNumber = f.PageNumber,
                PageSize = f.PageSize
            };


        }

        public async Task ProcessJoinRequestAsync(ProcessJoinRequestDto dto)
        {
            var approver = await _context.EmployerProfiles
                       .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId);
               if (approver == null)
                    throw new Exception("Employer profile not found.");

            var joinRequest = await _context.CompanyJoinRequests
                  .Include(r => r.EmployerProfile)
                   .FirstOrDefaultAsync(r => r.Id == dto.RequestId);

            if (joinRequest == null)
                throw new Exception("Join request not found.");

            if (approver.CompanyId != joinRequest.CompanyId
                || !_permissionService.HasPermission(approver, EmployerPermissions.ApproveJoinRequests))
                throw new Exception("Unauthorized to manage join requests for this company.");

            if (joinRequest.Status != JoinRequestStatus.Pending)
                throw new Exception("This join request has already been processed.");



            if (dto.IsApproved)
            {
                if (joinRequest.EmployerProfile.CompanyId != null)
                    throw new Exception("This employer is already linked to a company.");

                joinRequest.Status = JoinRequestStatus.Approved;
                joinRequest.EmployerProfile.CompanyId = joinRequest.CompanyId;
                joinRequest.EmployerProfile.Role = EmployerCompanyRole.Employer;
                joinRequest.EmployerProfile.Permissions = EmployerCompanyRole.Employer.DefaultPermissions();
                joinRequest.EmployerProfile.JoinedCompanyAt = DateTime.UtcNow;
            }
            else
            {
                joinRequest.Status = JoinRequestStatus.Rejected;
            }

            joinRequest.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task RequestToJoinCompanyAsync(int companyId)
        {
            var employer = await _context.EmployerProfiles
                            .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId);
            if (employer == null)
                throw new Exception("Employer profile not found.");

            if (employer.CompanyId != null)
                throw new Exception("Employer is already linked to a company.");

            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
                throw new Exception("Target company does not exist.");
            var existingRequest = await _context.CompanyJoinRequests
                .FirstOrDefaultAsync(r => r.EmployerProfileId == employer.Id
                                       && r.CompanyId == companyId
                                       && r.Status == JoinRequestStatus.Pending);

            if (existingRequest != null)
                throw new Exception("You already have a pending join request for this company.");

            var request = new CompanyJoinRequest
            {
                EmployerProfileId = employer.Id,
                CompanyId = companyId,
                Status = JoinRequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };

            _context.CompanyJoinRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CompanySearchDto>> SearchCompaniesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<CompanySearchDto>();

            return await _context.Companies.Where(x => x.IsVerified && x.CompanyName.Contains(query))
            .Select(c => new CompanySearchDto
            {
                CompanyId = c.Id,
                CompanyName = c.CompanyName,
                Location = c.Location,
                Industry = c.Industry,
                IsVerified = c.IsVerified
            })
            .Take(10)
            .ToListAsync();
                
        }
    }
}
