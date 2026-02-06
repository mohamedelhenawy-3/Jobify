using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.JobSeeker;
using Application.DTOs.Skills;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.JobSeeker;
using CloudinaryDotNet;
using Domain.Entities;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Implementation
{
    public class JobSeekerService : IJobSeekerService
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        private readonly CloudinaryService _cloudinaryService;

        public JobSeekerService(
              AppDBContext context,
              CloudinaryService cloudinaryService,
              IHttpContextAccessor httpContextAccessor)
        {
            _db = context;
            _cloudinaryService = cloudinaryService;
            _httpaccessor = httpContextAccessor;
        }
        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<JobSeekerProfile> GetJobSeekerProfile(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.JobSeekers
                .Include(js => js.ProfileImage) 
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");

            return new JobSeekerProfile
            {
                PhoneNumber = profile.PhoneNumber,
                ResumeUrl = profile.ResumeUrl,
                Location = profile.Location,
                ProfileImage = profile.ProfileImage
            };
        }

        public async Task<MyProfileDto> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");
            var user =  _db.Users.Where(x => x.Id == userId);
            var jobSeeker = await _db.JobSeekers
                .Include(js => js.Applications)
                .Include(js => js.ProfileImage)
                .Include(js => js.Skills)
                .Include(x=>x.Resume)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            var resume = _db.Pdfs.Where(x => x.JobSeekerId == jobSeeker.Id).Select(x => x.PdfUrl).FirstOrDefault();

            return new MyProfileDto
            {
                    FullName =user.Select(x=>x.FullName).FirstOrDefault(),
                PhoneNumber = jobSeeker.PhoneNumber,
                Location = jobSeeker.Location,
                ProfileImage = jobSeeker.ProfileImage.ImageUrl,
                Headline = jobSeeker.Headline,
                Resume = resume,
                Summary = jobSeeker.Summary,
                Skills = jobSeeker.Skills
                .Select(s => new MySkills
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToList()
            };



        }

        public async Task<SuccessResponse> UpdateMyProfile(UpdateMyProfile updateprofiledto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            // Load FULL user entity
            var user = await _db.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            // Load profile with related data
            var profile = await _db.JobSeekers
                .Include(js => js.ProfileImage)
                .Include(js => js.Skills)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");

            // ✅ Update user full name properly
            if (!string.IsNullOrEmpty(updateprofiledto.FullName))
                user.FullName = updateprofiledto.FullName;

            if (!string.IsNullOrEmpty(updateprofiledto.PhoneNumber))
                profile.PhoneNumber = updateprofiledto.PhoneNumber;

            if (!string.IsNullOrEmpty(updateprofiledto.Location))
                profile.Location = updateprofiledto.Location;

            if (!string.IsNullOrEmpty(updateprofiledto.Headline))
                profile.Headline = updateprofiledto.Headline;

            if (!string.IsNullOrEmpty(updateprofiledto.Summary))
                profile.Summary = updateprofiledto.Summary;

            // ================= IMAGE =================
            if (updateprofiledto.ProfileImage != null)
            {
                if (profile.ProfileImage != null &&
                    !string.IsNullOrEmpty(profile.ProfileImage.PublicId))
                {
                    await _cloudinaryService.DeleteImageAsync(profile.ProfileImage.PublicId);
                }

                var (url, publicId) =
                    await _cloudinaryService.UploadImageAsync(updateprofiledto.ProfileImage);

                if (profile.ProfileImage == null)
                {
                    profile.ProfileImage = new ProfileImage
                    {
                        ImageUrl = url,
                        PublicId = publicId,
                        CreatedAt = DateTime.UtcNow,
                        JobSeeker = profile
                    };
                }
                else
                {
                    profile.ProfileImage.ImageUrl = url;
                    profile.ProfileImage.PublicId = publicId;
                    profile.ProfileImage.CreatedAt = DateTime.UtcNow;
                }
            }

            // ================= SKILLS =================
            if (updateprofiledto.Skills != null && updateprofiledto.Skills.Any())
            {
                var existingSkills = profile.Skills
                    .Select(s => s.Name.ToLower())
                    .ToList();

                var newSkills = updateprofiledto.Skills
                    .Where(s => !existingSkills.Contains(s.ToLower()))
                    .Select(s => new Skill
                    {
                        Name = s.Trim(),
                        JobSeeker = profile
                    }).ToList();

                foreach (var skill in newSkills)
                {
                    profile.Skills.Add(skill);
                }
            }

            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "User profile updated successfully."
            };
        }

        public async Task<SuccessResponse> UploadCv(UploadMyResume resumedto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var profile = await _db.JobSeekers
                .Include(js => js.Resume)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");

            if (resumedto.Resume == null)
                throw new Exception("No file uploaded");

            // Delete old resume if exists
            if (profile.Resume != null && !string.IsNullOrEmpty(profile.Resume.PublicId))
            {
                await _cloudinaryService.DeleteImageAsync(profile.Resume.PublicId);
            }

            // Upload new PDF
            var (url, publicId) = await _cloudinaryService.UploadPdfAsync(resumedto.Resume);

            if (profile.Resume == null)
            {
                // 🔥 Create new Pdf entity
                var resume = new Pdf
                {
                    PdfUrl = url,
                    PublicId = publicId,
                    CreatedAt = DateTime.UtcNow,
                    JobSeekerId = profile.Id
                };

                profile.Resume = resume;

                await _db.Pdfs.AddAsync(resume); // 🔥 Important
            }
            else
            {
                // 🔥 Update existing one
                profile.Resume.PdfUrl = url;
                profile.Resume.PublicId = publicId;
                profile.Resume.CreatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "CV uploaded successfully."
            };
        }


        public async Task<ResumeDto> ViewMyCv()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var jobSeeker = await _db.JobSeekers
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
                throw new Exception("JobSeeker profile not found");

            var pdf = await _db.Pdfs
                .FirstOrDefaultAsync(p => p.JobSeekerId == jobSeeker.Id);

            if (pdf == null)
                throw new Exception("No CV uploaded yet");

            var resume = new ResumeDto
            {
                PdfUrl = pdf.PdfUrl // or pdf.PdfUrl if you want the full URL
            };
            return resume;
        }
    }
}
