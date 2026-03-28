using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny;
using Application.DTOs.Employer;
using Application.Interfaces.Employer;
using Domain.Entities;
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

namespace Infrastructure.Implementation.EmployerService
{
    public class EmployerServicee : IEmployer
    {
        private readonly ILogger<EmployerServicee> _log;
        private readonly AppDBContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployerServicee(
            AppDBContext context,
            CloudinaryService cloudinaryService,
            IHttpContextAccessor httpContextAccessor,ILogger<EmployerServicee> log)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;
            _log = log;
        }
        private string GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        public async Task<EmployerProfileResponseDto> MyProfile()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _context.EmployerProfiles.Include(x => x.User)
                .Include(x => x.ProfileImage)
                .Include(x => x.Company).ThenInclude(x => x.Logo)
                .Include(x => x.Company).ThenInclude(x => x.JobPosts)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (profile == null)
                throw new Exception("Employer profile not found");
            return new EmployerProfileResponseDto
            {
                FullName = profile.User.FullName,
                Email = profile.User.Email,
                PhoneNumber = profile.User.PhoneNumber,
                Location = profile.User.Location,
                LinkedInUrl = profile.User.LinkedInUrl,
                Position = profile.Position,
                Bio = profile.Bio,
                ProfileImageUrl = profile.ProfileImage?.ImageUrl
                ,
                Company=profile.Company == null ? null : new CompanyDto
                {
                    Id = profile.Company.Id,
                    CompanyName = profile.Company.CompanyName,
                    CompanyWebsite = profile.Company.CompanyWebsite,
                    Description = profile.Company.Description,
                    Industry = profile.Company.Industry,
                    Location = profile.Company.Location,
                    CompanySize = profile.Company.CompanySize,
                    Founded = profile.Company.Founded,
                    LogoUrl = profile.Company.Logo?.ImageUrl,
                    TotalJobs =profile.Company.JobPosts?.Count ?? 0,
                    ActiveJobs=profile.Company.JobPosts.Count(x=>x.isActive)
                
                }

            };
            }

        public async Task<SuccessResponseDto> UpdateAsync(UpdateEmployerProfile model)
        {
            var userId = GetCurrentUserId();
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
                user.FullName = model.FullName;

            if (!string.IsNullOrWhiteSpace(model.PhoneNumber))
                user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(model.Location))
                user.Location = model.Location;

            if (!string.IsNullOrWhiteSpace(model.LinkedInUrl))
                user.LinkedInUrl = model.LinkedInUrl;
            if (!string.IsNullOrWhiteSpace(model.Bio))
                profile.Bio = model.Bio;

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

            _context.EmployerProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return new SuccessResponseDto
            {
                Message = "Company profile updated successfully."
            };
        }
    }
}
