using Application.DTOs.Comapny;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Company;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.CompanyService
{
    public class CompanyService : ICompanyService
    {
        private ILogger<CompanyService> _log;
        private IHttpContextAccessor _httpContextAccessor;
        private AppDBContext _db;
        private CloudinaryService _cloudinaryService;
        public CompanyService(
          AppDBContext db,
          CloudinaryService cloudinaryService,
          IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        private async Task<EmployerProfile> GetCurrentEmployerProfile()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.EmployerProfiles
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (profile == null)
                throw new Exception("Employer profile not found");

            return profile;
        }

        public async Task<SuccessResponse> CreateCompany(CreateCompanyDto dto)
        {
            var profile = await GetCurrentEmployerProfile();
            var existingCompany = await _db.Companies
              .AnyAsync(c => c.EmployerProfileId == profile.Id);

            if(existingCompany) throw new Exception("You already have a company.");
            var company = new Company
            {
                CompanyName = dto.CompanyName,
                CompanyWebsite = dto.CompanyWebsite,
                Description = dto.Description,
                Location = dto.Location,
                Industry = dto.Industry,
                CompanySize = dto.CompanySize,
                Founded = dto.Founded,
                EmployerProfileId = profile.Id,
                CreatedAt = DateTime.UtcNow
            };

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();
            if (dto.Logo != null)
            {
                var (url, publicId) = await _cloudinaryService.UploadImageAsync(dto.Logo);

                var logo = new ProfileImage
                {
                    ImageUrl = url,
                    PublicId = publicId,
                    CreatedAt = DateTime.UtcNow,
                    CompanyId = company.Id
                };

                _db.ProfileImages.Add(logo);
                await _db.SaveChangesAsync();
            }

            return new SuccessResponse
            {
                Message = "Company Created Succefully"
            };
        }

        public async Task<SuccessResponse> DeleteCompany(int companyId)
        {

            var profile = await GetCurrentEmployerProfile();

            var company = await _db.Companies
                .Include(c => c.Logo)
                .FirstOrDefaultAsync(c => c.Id == companyId
                    && c.EmployerProfileId == profile.Id);

            if (company == null)
                throw new Exception("Company not found or you don't have permission.");

            // Delete logo from cloudinary
            if (company.Logo != null &&
                !string.IsNullOrEmpty(company.Logo.PublicId))
            {
                await _cloudinaryService.DeleteImageAsync(company.Logo.PublicId);
            }
            _db.Companies.Remove(company);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Company deleted successfully."
            };
        }

        public async Task<IEnumerable<CompanyResponseDto>> GetAllCompanies()
        {
            var companies = await _db.Companies
                           .Include(c => c.Logo)
                           .ToListAsync();

            return companies.Select(MapToResponseDto);
        }

        public async  Task<CompanyResponseDto> GetCompanyById(int companyId)
        {
            var company = await _db.Companies
                          .Include(c => c.Logo)
                          .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                throw new Exception("Company not found.");

            return MapToResponseDto(company);
        }

        public async Task<CompanyResponseDto> GetMyCompany()
        {
            var profile = await GetCurrentEmployerProfile();

            var company = await _db.Companies
                .Include(c => c.Logo)
                .FirstOrDefaultAsync(c => c.EmployerProfileId == profile.Id);

            if (company == null)
                throw new Exception("You don't have a company yet.");

            return MapToResponseDto(company);
        }

        public async  Task<SuccessResponse> UpdateCompany(int companyId, UpdateCompanyDto dto)
        {

            var profile = await GetCurrentEmployerProfile();

            var company = await _db.Companies
                .Include(c => c.Logo)
                .FirstOrDefaultAsync(c => c.Id == companyId
                    && c.EmployerProfileId == profile.Id);

            if (company == null)
                throw new Exception("Company not found or you don't have permission.");

            // Update fields only if provided
            if (!string.IsNullOrWhiteSpace(dto.CompanyName))
                company.CompanyName = dto.CompanyName;

            if (!string.IsNullOrWhiteSpace(dto.CompanyWebsite))
                company.CompanyWebsite = dto.CompanyWebsite;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                company.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Location))
                company.Location = dto.Location;

            if (!string.IsNullOrWhiteSpace(dto.Industry))
                company.Industry = dto.Industry;

            if (!string.IsNullOrWhiteSpace(dto.CompanySize))
                company.CompanySize = dto.CompanySize;

            if (!string.IsNullOrWhiteSpace(dto.Founded))
                company.Founded = dto.Founded;

            if (dto.Logo != null)
            {
                // Delete old logo from cloudinary
                if (company.Logo != null &&
                    !string.IsNullOrEmpty(company.Logo.PublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(company.Logo.PublicId);
                }

                var (url, publicId) = await _cloudinaryService.UploadImageAsync(dto.Logo);

                if (company.Logo == null)
                {
                    company.Logo = new ProfileImage
                    {
                        ImageUrl = url,
                        PublicId = publicId,
                        CreatedAt = DateTime.UtcNow,
                        CompanyId = company.Id
                    };
                }
                else
                {
                    company.Logo.ImageUrl = url;
                    company.Logo.PublicId = publicId;
                    company.Logo.CreatedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();

             return new SuccessResponse
            {
                Message = "Company deleted successfully."
            };

        }
      


        private CompanyResponseDto MapToResponseDto(Company company)
        {
            return new CompanyResponseDto
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                CompanyWebsite = company.CompanyWebsite,
                Description = company.Description,
                Location = company.Location,
                Industry = company.Industry,
                CompanySize = company.CompanySize,
                Founded = company.Founded,
                LogoUrl = company.Logo?.ImageUrl,
                CreatedAt = company.CreatedAt
            };
        }

        public async Task<CompanyOwnerDto> PrivateCompany()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.EmployerProfiles
                .FirstOrDefaultAsync(e => e.UserId == userId);

            if (profile == null)
                throw new Exception("Employer profile not found");
            var company = await _db.Companies
    .Include(c => c.Logo)
    .Include(c => c.JobPosts)
        .ThenInclude(j => j.Applications)
    .FirstOrDefaultAsync(c => c.EmployerProfileId == profile.Id);

            if (company == null)
                throw new Exception("Company not found");

            var allApplications = company.JobPosts
     .SelectMany(j => j.Applications)
     .ToList();

            return new CompanyOwnerDto
            {
                // ===== PUBLIC =====
                Id = company.Id,
                CompanyName = company.CompanyName,
                CompanyWebsite = company.CompanyWebsite,
                Description = company.Description,
                Industry = company.Industry,
                Location = company.Location,
                CompanySize = company.CompanySize,
                Founded = company.Founded,
                LogoUrl = company.Logo?.ImageUrl,
                CreatedAt = company.CreatedAt,

                // ===== PRIVATE STATS =====
                TotalJobsPosted = company.JobPosts.Count,
                ActiveJobs = company.JobPosts
          .Count(j => j.isActive == true),
                DraftJobs = company.JobPosts
          .Count(j => j.StatusId == 2),
                ClosedJobs = company.JobPosts
          .Count(j => j.StatusId == 3),

                TotalApplicationsReceived = allApplications.Count,
                PendingApplications = allApplications
          .Count(a => a.Status == ApplicationStatus.Applied),
                AcceptedApplications = allApplications
          .Count(a => a.Status == ApplicationStatus.Shortlisted),
                RejectedApplications = allApplications
          .Count(a => a.Status == ApplicationStatus.Rejected),
                InterviewScheduled = allApplications
          .Count(a => a.Status == ApplicationStatus.Interviewed)
            };
        }
    }
}

