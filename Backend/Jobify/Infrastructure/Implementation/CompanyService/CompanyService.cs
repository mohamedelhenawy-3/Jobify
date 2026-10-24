using Application.DTOs.Comapny;
using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.Comapny.RefactorCompany.RecordDto;
using Application.DTOs.JobSeeker;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Company;
using Application.SearchFeature.Dtos;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Domain.Entities.LooksUp.EmployerPermisions;
using Domain.Entities.LooksUp.JobEnum;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.CLoudinary;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.CompanyService
{
    public class CompanyService : ICompanyService
    {
        private ILogger<CompanyService> _log;
        private ICurrentUser _user;
        private AppDBContext _db;
        private CloudinaryService _cloudinaryService;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
        private readonly IMemoryCache _cache;

        public CompanyService(
            IMemoryCache cache,
            AppDBContext db,
            CloudinaryService cloudinaryService,
            ICurrentUser user)
        {
            _db = db;
            _cache = cache;
            _cloudinaryService = cloudinaryService;
            _user = user;
        }

        //public async Task<SuccessResponse> DeleteCompany(int companyId)
        //{
        //    var profile = await GetCurrentEmployerProfile();

        //    var company = await _db.Companies
        //        .Include(c => c.Logo)
        //        .FirstOrDefaultAsync(c => c.Id == companyId
        //            && c.EmployerProfileId == profile.Id);

        //    if (company == null)
        //        throw new Exception("Company not found or you don't have permission.");

        //    // Delete logo from cloudinary
        //    if (company.Logo != null &&
        //        !string.IsNullOrEmpty(company.Logo.PublicId))
        //    {
        //        await _cloudinaryService.DeleteImageAsync(company.Logo.PublicId);
        //    }
        //    _db.Companies.Remove(company);
        //    await _db.SaveChangesAsync();

        //    return new SuccessResponse
        //    {
        //        Message = "Company deleted successfully."
        //    };
        //}

        public async Task<IEnumerable<CompanyResponseDto>> GetAllCompanies()
        {
            var companies = await _db.Companies
                .AsNoTracking()
                .Include(c => c.Logo)
                .Include(c => c.Industry)   // needed for MapToResponseDto
                .ToListAsync();

            return companies.Select(MapToResponseDto);
        }

        public async Task<CompanyResponseDto> GetCompanyById(int companyId)
        {
            var company = await _db.Companies
                .Include(c => c.Logo)
                .Include(c => c.Industry)   // needed for MapToResponseDto
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                throw new Exception("Company not found.");

            return MapToResponseDto(company);
        }

        //public async Task<CompanyResponseDto> GetMyCompany()
        //{
        //    var profile = await GetCurrentEmployerProfile();

        //    var company = await _db.Companies
        //        .Include(c => c.Logo)
        //        .Include(c => c.Industry)
        //        .FirstOrDefaultAsync(c => c.EmployerProfileId == profile.Id);

        //    if (company == null)
        //        throw new Exception("You don't have a company yet.");

        //    return MapToResponseDto(company);
        //}

        //public async Task<SuccessResponse> UpdateCompany(int companyId, UpdateCompanyDto dto)
        //{
        //    var profile = await GetCurrentEmployerProfile();

        //    var company = await _db.Companies
        //        .Include(c => c.Logo)
        //        .FirstOrDefaultAsync(c => c.Id == companyId
        //            && c.EmployerProfileId == profile.Id);

        //    if (company == null)
        //        throw new Exception("Company not found or you don't have permission.");

        //    // Update fields only if provided
        //    if (!string.IsNullOrWhiteSpace(dto.CompanyName))
        //        company.CompanyName = dto.CompanyName;

        //    if (!string.IsNullOrWhiteSpace(dto.CompanyWebsite))
        //        company.CompanyWebsite = dto.CompanyWebsite;

        //    if (!string.IsNullOrWhiteSpace(dto.Description))
        //        company.Description = dto.Description;

        //    if (!string.IsNullOrWhiteSpace(dto.Location))
        //        company.Location = dto.Location;

        //    // Industry is now an id, validated against active industries
        //    if (dto.IndustryId.HasValue)
        //    {
        //        var ok = await _db.Industries.AnyAsync(i => i.Id == dto.IndustryId && i.IsActive);
        //        if (!ok) throw new Exception("Invalid industry.");
        //        company.IndustryId = dto.IndustryId;
        //    }

        //    if (!string.IsNullOrWhiteSpace(dto.CompanySize))
        //        company.CompanySize = dto.CompanySize;

        //    if (!string.IsNullOrWhiteSpace(dto.Founded))
        //        company.Founded = dto.Founded;

        //    if (dto.Logo != null)
        //    {
        //        // Delete old logo from cloudinary
        //        if (company.Logo != null &&
        //            !string.IsNullOrEmpty(company.Logo.PublicId))
        //        {
        //            await _cloudinaryService.DeleteImageAsync(company.Logo.PublicId);
        //        }

        //        var (url, publicId) = await _cloudinaryService.UploadImageAsync(dto.Logo);

        //        if (company.Logo == null)
        //        {
        //            company.Logo = new ProfileImage
        //            {
        //                ImageUrl = url,
        //                PublicId = publicId,
        //                CreatedAt = DateTime.UtcNow,
        //                CompanyId = company.Id
        //            };
        //        }
        //        else
        //        {
        //            company.Logo.ImageUrl = url;
        //            company.Logo.PublicId = publicId;
        //            company.Logo.CreatedAt = DateTime.UtcNow;
        //        }
        //    }

        //    await _db.SaveChangesAsync();

        //    return new SuccessResponse
        //    {
        //        Message = "Company updated successfully."
        //    };
        //}

        private CompanyResponseDto MapToResponseDto(Company company)
        {
            return new CompanyResponseDto
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                CompanyWebsite = company.CompanyWebsite,
                Description = company.Description,
                Location = company.Location,
                IndustryId = company.IndustryId,       // add this property to CompanyResponseDto (int?)
                Industry = company.Industry?.Name,     // name for display (string?)
                CompanySize = company.CompanySize,
                Founded = company.Founded,
                LogoUrl = company.Logo?.ImageUrl,
                CreatedAt = company.CreatedAt
            };
        }

        //    public async Task<CompanyOwnerDto> PrivateCompany()
        //    {
        //        var userId = GetCurrentUserId();
        //        if (string.IsNullOrEmpty(userId))
        //            throw new Exception("Unauthorized");

        //        var profile = await _db.EmployerProfiles
        //            .FirstOrDefaultAsync(e => e.UserId == userId);

        //        if (profile == null)
        //            throw new Exception("Employer profile not found");

        //        var company = await _db.Companies
        //            .Include(c => c.Logo)
        //            .Include(c => c.Industry)
        //            .Include(c => c.JobPosts)
        //                .ThenInclude(j => j.Applications)
        //            .FirstOrDefaultAsync(c => c.EmployerProfileId == profile.Id);

        //        if (company == null)
        //            throw new Exception("Company not found");

        //        var allApplications = company.JobPosts
        //            .SelectMany(j => j.Applications)
        //            .ToList();

        //        return new CompanyOwnerDto
        //        {
        //            // ===== PUBLIC =====
        //            Id = company.Id,
        //            CompanyName = company.CompanyName,
        //            CompanyWebsite = company.CompanyWebsite,
        //            Description = company.Description,
        //            Industry = company.Industry?.Name,
        //            Location = company.Location,
        //            CompanySize = company.CompanySize,
        //            Founded = company.Founded,
        //            LogoUrl = company.Logo?.ImageUrl,
        //            CreatedAt = company.CreatedAt,

        //            // ===== PRIVATE STATS =====
        //            TotalJobsPosted = company.JobPosts.Count,
        //            ActiveJobs = company.JobPosts.Count(j => j.isActive == true),
        //            DraftJobs = company.JobPosts.Count(j => j.StatusId == 2),
        //            ClosedJobs = company.JobPosts.Count(j => j.StatusId == 3),

        //            TotalApplicationsReceived = allApplications.Count,
        //            PendingApplications = allApplications.Count(a => a.Status == ApplicationStatus.Applied),
        //            AcceptedApplications = allApplications.Count(a => a.Status == ApplicationStatus.Shortlisted),
        //            RejectedApplications = allApplications.Count(a => a.Status == ApplicationStatus.Rejected),
        //            InterviewScheduled = allApplications.Count(a => a.Status == ApplicationStatus.Interviewed)
        //        };
        //    }

        public async Task<List<SearchCompanyDto>> SearchCompaniesAsync(GlobalSearchDto dto)
        {
            var query = _db.Companies
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Keyword))
            {
                var keyword = dto.Keyword.Trim().ToLower();
                query = query.Where(c =>
                    c.CompanyName.ToLower().Contains(keyword) ||
                    (c.Industry != null && c.Industry.Name.ToLower().Contains(keyword)));
            }

            // ✅ filter by the selected industry (the id from the dropdown)
            if (dto.IndustryId.HasValue)
                query = query.Where(c => c.IndustryId == dto.IndustryId);

          

            if (!string.IsNullOrWhiteSpace(dto.CompanySize))
                query = query.Where(c => c.CompanySize == dto.CompanySize);

            if (!string.IsNullOrWhiteSpace(dto.Location))
            {
                var location = dto.Location.Trim().ToLower();
                query = query.Where(c => c.Location != null && c.Location.ToLower().Contains(location));
            }

            return await query
                .OrderByDescending(c => c.IsVerified)
                .ThenBy(c => c.CompanyName)
                .Take(50)
                .Select(c => new SearchCompanyDto
                {
                    Id = c.Id,
                    Logo = c.Logo != null ? c.Logo.ImageUrl : null,
                    CompanyName = c.CompanyName,
                    Industry = c.Industry != null ? c.Industry.Name : null,
                    Location = c.Location,
                    Description = c.Description,
                    CompanySize = c.CompanySize,
                    IsVerified = c.IsVerified
                })
                .ToListAsync();
        }
        public Task<CompanyOwnerDto> PrivateCompany()
        {
            throw new NotImplementedException();
        }

        public Task<CompanyResponseDto> GetMyCompany()
        {
            throw new NotImplementedException();
        }

        public Task<PublicCompanyDto> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<HiringSnapshotDto?> GetHiringSnapshotAsync()
        {
            var me = await _db.EmployerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == _user.UserId);
            if (me?.CompanyId == null)
                return null;

            var companyId = me.CompanyId;
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);
            var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var applications = _db.JobApplications
                .AsNoTracking()
                .Where(a => a.JobPost.CompanyId == companyId);

            // run one after another because they share a single DbContext
            var openJobs = await _db.JobPosts
                .CountAsync(j => j.CompanyId == companyId && j.IsActive);

            var newThisWeek = await applications
                .CountAsync(a => a.AppliedAt >= weekAgo);

            var interviews = await applications
                .CountAsync(a => a.Status == ApplicationStatus.Interviewed);

            var hires = await applications
                .CountAsync(a => a.Status == ApplicationStatus.Hired);

            return new HiringSnapshotDto
            {
                OpenJobs = openJobs,
                NewApplicantsThisWeek = newThisWeek,
                InterviewsScheduled = interviews,
                HiresThisMonth = hires
            };
        }

        public async Task<IReadOnlyList<TopHiringCompanyDto>> GetTopHiringCompaniesAsync(int count = 8, CancellationToken ct = default)
        {
            count = Math.Clamp(count, 1, 24);

            var cacheKey = $"top_hiring_companies_{count}";
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<TopHiringCompanyDto>? cached) && cached is not null)
                return cached;

            // No Include needed: Select projections load what they use
            var companies = await _db.Companies
                .AsNoTracking()
                .Where(c => c.JobPosts.Any(j => j.IsActive))
                .Select(c => new
                {
                    Company = c,
                    OpenJobs = c.JobPosts.Count(j => j.IsActive)
                })
                .OrderByDescending(x => x.OpenJobs)
                .ThenBy(x => x.Company.CompanyName)
                .Take(count)
                .Select(x => new TopHiringCompanyDto(
                    x.Company.Id,
                    x.Company.CompanyName,
                    x.Company.Logo != null ? x.Company.Logo.ImageUrl : null,
                    x.Company.Industry != null ? x.Company.Industry.Name : null,
                    x.Company.Location,
                    x.OpenJobs))
                .ToListAsync(ct);

            _cache.Set(cacheKey, companies, CacheDuration);
            return companies;
        }
    }
}