using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny;
using Application.DTOs.Employer;
using Application.Interfaces.Employer;
using Application.Interfaces.Permisions;
using Application.Interfaces.User;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Domain.Entities.LooksUp.EmployerPermisions;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Reposoitory.UserRepository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.EmployerService
{
    public class EmployerServicee : IEmployer
    {
        private readonly ILogger<EmployerServicee> _log;
        private readonly AppDBContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly ICurrentUser _currentUserService;
        private readonly IEmployerPermissionService _Ipermision;


        public EmployerServicee(
            AppDBContext context,
            CloudinaryService cloudinaryService,
            ICurrentUser currentUserService, ILogger<EmployerServicee> log, IEmployerPermissionService Ipermision)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _log = log;
            _currentUserService = currentUserService;
            _Ipermision = Ipermision;
        }

        public async Task<EmployerProfileDto?> GetOwnerProfileAsync()
        {
            var employer = await _Ipermision.GetCurrentEmployerAsync();
            if (employer?.User == null || employer.Company == null)
                return null;

            var canSeeCompanyWide = employer.Permissions.Contains(EmployerPermissions.ViewAnalytics)
                                  || employer.Role is EmployerCompanyRole.Owner or EmployerCompanyRole.Admin;

            int jobsPostedCount;
            int activeJobsCount;
            int totalApplicants;

            if (canSeeCompanyWide)
            {
                jobsPostedCount = await _context.JobPosts.CountAsync(j => j.CompanyId == employer.CompanyId);
                activeJobsCount = await _context.JobPosts.CountAsync(j => j.CompanyId == employer.CompanyId && j.IsActive);
                totalApplicants = await _context.JobApplications.CountAsync(a => a.JobPost!.CompanyId == employer.CompanyId);
            }
            else
            {
                jobsPostedCount = await _context.JobPosts.CountAsync(j => j.PostedByEmployerId == employer.Id);
                activeJobsCount = await _context.JobPosts.CountAsync(j => j.PostedByEmployerId == employer.Id && j.IsActive);
                totalApplicants = await _context.JobApplications.CountAsync(a => a.JobPost!.PostedByEmployerId == employer.Id);
            }

            // ✅ read the industry name directly, so it works even if
            // GetCurrentEmployerAsync() doesn't include Company.Industry
            var industryName = await _context.Companies
                .AsNoTracking()
                .Where(c => c.Id == employer.CompanyId)
                .Select(c => c.Industry != null ? c.Industry.Name : null)
                .FirstOrDefaultAsync();

            return new EmployerProfileDto
            {
                FullName = employer.User.FullName,
                LinkedIn = employer.User.LinkedInUrl,
                position = employer.Position,
                Bio = employer.Bio,
                Email = employer.User.Email,
                Phone = employer.User.PhoneNumber,
                Location = employer.User.Location,
                PhotoUrl = employer.ProfileImage?.ImageUrl,
                CompanyName = employer.Company.CompanyName,
                CompanyIndustry = industryName,          // ✅ was employer.Company.Industry
                CompanyWebsite = employer.Company.CompanyWebsite,
                JobsPostedCount = jobsPostedCount,
                ActiveJobsCount = activeJobsCount,
                TotalApplicants = totalApplicants,
                Role = employer.Role.ToString(),
                Permissions = employer.Permissions
            };
        }


        public async Task<SuccessResponseDto> UpdateAsync(UpdateEmployerProfile model)
        {
            var userId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            var profile = await _context.EmployerProfiles
                .Include(c => c.ProfileImage)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (profile == null)
                throw new Exception("Employer profile not found");
            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName ?? user.FullName;

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(model.Location))
                user.Location = model.Location ?? user.Location;

            if (!string.IsNullOrWhiteSpace(model.Position))
                profile.Position = model.Position ?? profile.Position;

            if (!string.IsNullOrWhiteSpace(model.LinkedIn))
                user.LinkedInUrl = model.LinkedIn;
            if (!string.IsNullOrWhiteSpace(model.Bio))
                profile.Bio = model.Bio ?? profile.Bio;

            user.UpdatedAt = DateTime.UtcNow;


            if (model.PImage != null)
            {
                if (profile.ProfileImage != null && !string.IsNullOrEmpty(profile.ProfileImage.PublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(profile.ProfileImage.PublicId);
                }

                var (url, publicId) = await _cloudinaryService.UploadImageAsync(model.PImage);

                if (profile.ProfileImage == null)
                {
                    profile.ProfileImage = new ProfileImage
                    {
                        ImageUrl = url,
                        PublicId = publicId,
                        CreatedAt = DateTime.UtcNow,
                        EmployerProfile = profile
                    };
                }
                else
                {
                    profile.ProfileImage.ImageUrl = url;
                    profile.ProfileImage.PublicId = publicId;
                    profile.ProfileImage.CreatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return new SuccessResponseDto
            {
                Message = "User profile updated successfully."
            };
        }

        public Task<bool> UpdateProfileAsync(UpdateEmployerProfile dto)
        {
            throw new NotImplementedException();
        }
    }
}