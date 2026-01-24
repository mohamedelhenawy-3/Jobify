using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Employer;
using Application.Interfaces.Company;
using Domain.Entities;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Implementation.CompanyService
{
    public class EmployerService : IEmployerService
    {
        private readonly AppDBContext _context;
        private readonly CloudinaryService _cloudinaryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployerService(
            AppDBContext context,
            CloudinaryService cloudinaryService,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<List<EmployerProfile>> GetAllAsync()
        {
            return await _context.EmployerProfiles.Include(c => c.JobPosts).ToListAsync();
        }

        public async Task<EmployerProfile?> GetByIdAsync(int id)
        {
            return await _context.EmployerProfiles
                .Include(c => c.JobPosts)
                .Include(c => c.ProfileImage)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<EmployerProfile?> GetByCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId)) return null;

            return await _context.EmployerProfiles
                .Include(c => c.JobPosts)
                .Include(c => c.ProfileImage)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<SuccessResponseDto> UpdateAsync(UpdateEmployerProfile model)
        {
            // Get the current logged-in user's profile
            var profile = await GetByCurrentUserAsync();
            if (profile == null)
            {
                return new SuccessResponseDto
                {
                    Message = "Company profile not found."
                };
            }

            // Update fields if provided
            if (!string.IsNullOrEmpty(model.CompanyName)) profile.CompanyName = model.CompanyName;
            if (!string.IsNullOrEmpty(model.CompanyWebsite)) profile.CompanyWebsite = model.CompanyWebsite;
            if (!string.IsNullOrEmpty(model.Description)) profile.Description = model.Description;
            if (!string.IsNullOrEmpty(model.Location)) profile.Location = model.Location;

            // Handle profile image
            if (model.ProfileImage != null)
            {
                // Delete old image if exists
                if (profile.ProfileImage != null && !string.IsNullOrEmpty(profile.ProfileImage.PublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(profile.ProfileImage.PublicId);
                }

                // Upload new image
                var (url, publicId) = await _cloudinaryService.UploadImageAsync(model.ProfileImage);

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

            // Save changes
            _context.EmployerProfiles.Update(profile);
            await _context.SaveChangesAsync();

            return new SuccessResponseDto
            {
                Message = "Company profile updated successfully."
            };
        }

        public Task<EmployerProfile?> GetByEmployerIdAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
