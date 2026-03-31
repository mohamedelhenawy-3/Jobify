using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Education;
using Application.DTOs.Experience;
using Application.DTOs.JobSeeker;
using Application.DTOs.Skills;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.JobSeeker;
using CloudinaryDotNet;
using Domain.Entities;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Implementation.FileUploadService;
using Infrastructure.Reposoitory.UserRepository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly SupabaseService _supabaseservice;

        public JobSeekerService(
              AppDBContext context,
              CloudinaryService cloudinaryService,
              IHttpContextAccessor httpContextAccessor,
               SupabaseService supabaseservice

              )
        {
            _db = context;
            _cloudinaryService = cloudinaryService;
            _httpaccessor = httpContextAccessor;
            _supabaseservice = supabaseservice;
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
                .Include(js => js.User)
                .Include(js => js.ProfileImage)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");

            return new JobSeekerProfile
            {
                PhoneNumber = profile.User.PhoneNumber,
                ResumeUrl = profile.ResumeUrl,
                Location = profile.User.Location,
                ProfileImage = profile.ProfileImage
            };
        }


        public async Task<MyProfileDto> GetMyProfile()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var jobSeeker = await _db.JobSeekers
                .Include(js => js.User)
                .Include(js => js.ProfileImage)
                .Include(js => js.Skills)
                .Include(js => js.Experiences)
                .Include(js => js.Educations)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (jobSeeker == null)
                throw new Exception("JobSeeker profile not found");

            var resume = _db.Pdfs
                .Where(x => x.JobSeekerId == jobSeeker.Id)
                .Select(x => x.PdfUrl)
                .FirstOrDefault();

            return new MyProfileDto
            {
                FullName = jobSeeker.User.FullName,
                PhoneNumber = jobSeeker.User.PhoneNumber,
                Location = jobSeeker.User.Location,
                LinkedInUrl = jobSeeker.User.LinkedInUrl,
                ProfileImage = jobSeeker.ProfileImage?.ImageUrl,
                Headline = jobSeeker.Headline,
                Summary = jobSeeker.Summary,
                GithubUrl = jobSeeker.GithubUrl,
                ResumeUrl = jobSeeker.ResumeUrl,
                Resume = resume,
                Skills = jobSeeker.Skills.Select(s => new MySkills
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList(),
                Experiences = jobSeeker.Experiences?.Select(e => new ExperienceDto
                {
                    Id = e.Id,
                    CompanyName = e.CompanyName,
                    JobTitle = e.JobTitle,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                }).ToList() ?? new List<ExperienceDto>(),
                Educations = jobSeeker.Educations?.Select(e => new EducationDto
                {
                    Id = e.Id,
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                }).ToList() ?? new List<EducationDto>()
            };
        }

        public async Task<SuccessResponse> UpdateMyProfile(UpdateMyProfile updateprofiledto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
                throw new Exception("User not found");

            var profile = await _db.JobSeekers
                .Include(js => js.ProfileImage)
                .Include(js => js.Skills)
                .Include(js => js.Experiences)
                .Include(js => js.Educations)
                .FirstOrDefaultAsync(js => js.UserId == userId);

            if (profile == null)
                throw new Exception("JobSeeker profile not found");

            if (!string.IsNullOrWhiteSpace(updateprofiledto.FullName))
                user.FullName = updateprofiledto.FullName;

            if (!string.IsNullOrWhiteSpace(updateprofiledto.PhoneNumber))
                user.PhoneNumber = updateprofiledto.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(updateprofiledto.Location))
                user.Location = updateprofiledto.Location;

            if (!string.IsNullOrWhiteSpace(updateprofiledto.LinkedInUrl))
                user.LinkedInUrl = updateprofiledto.LinkedInUrl;

            user.UpdatedAt = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(updateprofiledto.Headline))
                profile.Headline = updateprofiledto.Headline;

            if (!string.IsNullOrWhiteSpace(updateprofiledto.GithubUrl))
                profile.GithubUrl = updateprofiledto.GithubUrl;

            if (!string.IsNullOrWhiteSpace(updateprofiledto.Summary))
                profile.Summary = updateprofiledto.Summary;

            if (updateprofiledto.ProfileImage != null)
            {
                if (profile.ProfileImage != null && !string.IsNullOrEmpty(profile.ProfileImage.PublicId))
                    await _cloudinaryService.DeleteImageAsync(profile.ProfileImage.PublicId);

                var (url, publicId) = await _cloudinaryService.UploadImageAsync(updateprofiledto.ProfileImage);

                if (profile.ProfileImage == null)
                {
                    profile.ProfileImage = new ProfileImage
                    {
                        ImageUrl = url,
                        PublicId = publicId,
                        CreatedAt = DateTime.UtcNow,
                        JobSeekerId = profile.Id
                    };
                }
                else
                {
                    profile.ProfileImage.ImageUrl = url;
                    profile.ProfileImage.PublicId = publicId;
                    profile.ProfileImage.CreatedAt = DateTime.UtcNow;
                }
            }

            if (updateprofiledto.Skills != null && updateprofiledto.Skills.Any())
            {
                var existingSkills = profile.Skills.Select(s => s.Name.ToLower()).ToList();
                var newSkills = updateprofiledto.Skills
                    .Where(s => !existingSkills.Contains(s.ToLower()))
                    .Select(s => new Skill
                    {
                        Name = s.Trim(),
                        JobSeekerId = profile.Id
                    }).ToList();

                foreach (var skill in newSkills)
                    profile.Skills.Add(skill);
            }

            List<EducationDto> educations = string.IsNullOrEmpty(updateprofiledto.EducationsJson)
? new List<EducationDto>()
: JsonConvert.DeserializeObject<List<EducationDto>>(updateprofiledto.EducationsJson)!;
            

            List<ExperienceDto> experiences = string.IsNullOrEmpty(updateprofiledto.ExperiencesJson)
                ? new List<ExperienceDto>()
                : JsonConvert.DeserializeObject<List<ExperienceDto>>(updateprofiledto.ExperiencesJson)!;

            // call your update methods
            UpdateEducations(profile, educations);
            UpdateExperiences(profile, experiences);

            await _db.SaveChangesAsync();

            return new SuccessResponse { Message = "Profile updated successfully." };
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
            var url = await _supabaseservice.UploadResume(resumedto.Resume);
            profile.ResumeUrl = url;

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

        private void UpdateExperiences(JobSeeker jobSeeker, List<ExperienceDto>? dtos)
        {
            dtos ??= new List<ExperienceDto>();

            jobSeeker.Experiences ??= new List<Experience>();

            var dtoIds = dtos
                .Where(x => x.Id != null)
                .Select(x => x.Id!.Value)
                .ToList();

            var removed = jobSeeker.Experiences
                .Where(e => !dtoIds.Contains(e.Id))
                .ToList();

            _db.Experiences.RemoveRange(removed);

            foreach (var dto in dtos)
            {
                if (dto.Id == null)
                {
                    // ADD new experience
                    jobSeeker.Experiences.Add(new Experience
                    {
                        CompanyName = dto.CompanyName,
                        JobTitle = dto.JobTitle,
                        Description = dto.Description,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate
                    });
                }
                else
                {
                    // UPDATE existing experience
                    var existing = jobSeeker.Experiences
                        .FirstOrDefault(e => e.Id == dto.Id);

                    if (existing != null)
                    {
                        existing.CompanyName = dto.CompanyName;
                        existing.JobTitle = dto.JobTitle;
                        existing.Description = dto.Description;
                        existing.StartDate = dto.StartDate;
                        existing.EndDate = dto.EndDate;
                    }
                }
            }
        }
        private void UpdateEducations(JobSeeker jobSeeker, List<EducationDto>? educationDtos)
        {
            educationDtos ??= new List<EducationDto>();

            jobSeeker.Educations ??= new List<Education>();

            // IDs coming from frontend
            var dtoIds = educationDtos
                .Where(e => e.Id.HasValue)
                .Select(e => e.Id!.Value)
                .ToList();

            // Find educations removed by the user
            var removedEducations = jobSeeker.Educations
                .Where(e => !dtoIds.Contains(e.Id))
                .ToList();

            _db.Educations.RemoveRange(removedEducations);

            foreach (var dto in educationDtos)
            {
                if (dto.Id == null)
                {
                    // ADD new education
                    jobSeeker.Educations.Add(new Education
                    {
                        Institution = dto.Institution,
                        Degree = dto.Degree,
                        FieldOfStudy = dto.FieldOfStudy,
                        StartDate = dto.StartDate,
                        EndDate = dto.EndDate
                    });
                }
                else
                {
                    // UPDATE existing education
                    var existingEducation = jobSeeker.Educations
                        .FirstOrDefault(e => e.Id == dto.Id);

                    if (existingEducation != null)
                    {
                        existingEducation.Institution = dto.Institution;
                        existingEducation.Degree = dto.Degree;
                        existingEducation.FieldOfStudy = dto.FieldOfStudy;
                        existingEducation.StartDate = dto.StartDate;
                        existingEducation.EndDate = dto.EndDate;
                    }
                }
            }
        }
        //private List<T> SafeDeserialize<T>(string? json)
        //{
        //    if (string.IsNullOrWhiteSpace(json)) return new List<T>();
        //    try
        //    {
        //        return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
        //    }
        //    catch
        //    {
        //        return new List<T>();
        //    }
        //}
    }


}
