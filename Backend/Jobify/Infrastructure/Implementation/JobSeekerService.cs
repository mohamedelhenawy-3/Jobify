using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Education;
using Application.DTOs.Experience;
using Application.DTOs.JobSeeker;
using Application.DTOs.JobSeeker.JobSeekers;
using Application.DTOs.JobSeeker.JobSeekers.Updates;
using Application.DTOs.Skills;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.JobSeeker;
using Application.SearchFeature.Dtos;
using CloudinaryDotNet;
using Domain.Entities;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Implementation.FileUploadService;
using Infrastructure.Reposoitory.UserRepository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly ICurrentUser _currentUser;
        private readonly IHttpContextAccessor _httpaccessor;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SupabaseService _supabaseservice;

        public JobSeekerService(
              ICurrentUser currentUser,
             AppDBContext context,
              CloudinaryService cloudinaryService,
              IHttpContextAccessor httpContextAccessor,
               SupabaseService supabaseservice

              )
        {
            _currentUser = currentUser;
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
        public async Task<SuccessResponse> RemoveCv()
        {
            var userId = GetCurrentUserId();
            var profile = await _db.JobSeekers
                .Include(js => js.Resume)
                .FirstOrDefaultAsync(js => js.UserId == userId)
                ?? throw new Exception("JobSeeker profile not found");

            var oldUrl = profile.ResumeUrl;

            if (profile.Resume != null) _db.Pdfs.Remove(profile.Resume);
            profile.ResumeUrl = null;
            await _db.SaveChangesAsync();

            await _supabaseservice.DeleteResume(oldUrl);   // delete the file after the database is updated

            return new SuccessResponse { Message = "CV removed." };
        }

        public async Task<SuccessResponse> UpdateProfilePrivacvy(UpdatePrivacyDto updateprivancy)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Unauthorized");

            var user = await _db.Users
                .FirstOrDefaultAsync(js => js.Id == userId);


            user.IsProfilePublic = updateprivancy.IsProfilePublic;
            await _db.SaveChangesAsync();
            return new SuccessResponse
            {
                Message = "Privacy Updated Succfully"
            };

        }

        public async Task<List<UserSeachDto>> SearchUsersAsync(GlobalSearchDto dto)
        {
            var query = _db.Users
      .AsNoTracking()
      .Include(u => u.JobSeeker)
          .ThenInclude(js => js.ProfileImage)
      .Include(u => u.EmployerProfile)
          .ThenInclude(e => e.ProfileImage)
      .AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Keyword))
            {
                var keyword = dto.Keyword.ToLower();

                query = query.Where(u =>
                    u.FullName.ToLower().Contains(keyword) ||

                    (u.JobSeeker != null &&
                        u.JobSeeker.Headline.ToLower().Contains(keyword)) ||

                    (u.EmployerProfile != null &&
                        u.EmployerProfile.Company.CompanyName.ToLower().Contains(keyword))
                );
            }

            return await query.Select(u => new UserSeachDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Location = u.Location,


                ProfileImageUrl =
                    u.JobSeeker != null
                        ? u.JobSeeker.ProfileImage.ImageUrl
                        : u.EmployerProfile != null
                            ? u.EmployerProfile.ProfileImage.ImageUrl
                            : null,

                Headline =
                    u.JobSeeker != null
                        ? u.JobSeeker.Headline
                        : u.EmployerProfile.Company.CompanyName,

                UserRole =
                    u.JobSeeker != null ? "JobSeeker" :
                    u.EmployerProfile != null ? "Employer" :
                    "Unknown"

            }).ToListAsync();


        }

        public Task<JobSeekerProfile> GetJobSeekerProfile(int id)
        {
            throw new NotImplementedException();
        }



        private async Task<JobSeeker> LoadAsync()
        {
            var userId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException("Log in first.");

            var p = await _db.JobSeekers
                .Include(x => x.User).Include(c => c.ProfileImage)
                .Include(x => x.Experiences).Include(x => x.Educations).Include(x => x.Skills)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (p == null)                                   // created on first visit
            {
                p = new JobSeeker { UserId = userId };
                _db.JobSeekers.Add(p);
                await _db.SaveChangesAsync();
                p.User = await _db.Users.FirstAsync(u => u.Id == userId);
            }
            return p;
        }
        private static string? Clean(string? s, int max)
        {
            s = s?.Trim();
            if (string.IsNullOrEmpty(s)) return null;
            return s.Length > max ? s[..max] : s;
        }

        private static string? CleanUrl(string? s)
        {
            s = Clean(s, 300);
            if (s == null) return null;
            if (!Uri.TryCreate(s, UriKind.Absolute, out var u) || (u.Scheme != "http" && u.Scheme != "https"))
                throw new ArgumentException("Links must start with http:// or https://");
            return s;
        }

        private async Task<ProfileDto> SaveAndReturnAsync(JobSeeker p)
        {
            p.User.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return ToDto(p);
        }
        private static ProfileDto ToDto(JobSeeker p)
        {
            var missing = new List<string>();
            int score = 0;
            void Check(bool ok, int points, string tip) { if (ok) score += points; else missing.Add(tip); }

            Check(p.ProfileImage?.ImageUrl != null, 10, "Add a profile photo");
            Check(!string.IsNullOrEmpty(p?.Headline), 10, "Add a headline");
            Check(!string.IsNullOrEmpty(p?.About), 10, "Write a short about section");
            Check(!string.IsNullOrEmpty(p.User?.Location), 5, "Add your location");
            Check(p.Experiences.Count > 0, 20, "Add your work experience");
            Check(p.Educations.Count > 0, 10, "Add your education");
            Check(p.Skills.Count >= 5, 15, "Add at least 5 skills");
            Check(!string.IsNullOrEmpty(p.Resume?.PdfUrl), 15, "Upload your CV");
            Check(!string.IsNullOrEmpty(p?.DesiredRole), 5, "Set your job preferences");

            return new ProfileDto
            {
                FullName = p.User?.FullName,
                Email = p.User.Email ?? "",
                PhotoUrl = p.ProfileImage?.ImageUrl,
                Headline = p.Headline,
                About = p?.About,
                ResumeUrl=p.Resume?.PdfUrl,
                Location = p.User?.Location,
                Phone = p.User?.PhoneNumber,
                LinkedInUrl = p.User.LinkedInUrl,
                GitHubUrl = p.GithubUrl,
                WebsiteUrl = p.WebsiteUrl,
                IsOpenToWork = p.IsOpenToWork,
                DesiredRole = p.DesiredRole,
                PreferredJobType = p.PreferredJobType,
                ExpectedSalary = p.ExpectedSalary,
                NoticePeriod = p.NoticePeriod,
                PreferredLocations = p.PreferredLocations,
                IsPublic = p.IsPublic,
                Username = p.Username,
                ShowEmail = p.ShowEmail,
                ShowPhone = p.ShowPhone,
                ShowCv = p.ShowCv,
                Experiences = p.Experiences.OrderByDescending(e => e.IsCurrent).ThenByDescending(e => e.StartDate)
                    .Select(e => new Application.DTOs.JobSeeker.JobSeekers.ExperienceDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Company = e.Company,
                        Location = e.Location,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        IsCurrent = e.IsCurrent,
                        Description = e.Description
                    }).ToList(),
                Educations = p.Educations.OrderByDescending(e => e.StartDate)
                    .Select(e => new Application.DTOs.JobSeeker.JobSeekers.EducationDto
                    {
                        Id = e.Id,
                        School = e.School,
                        Degree = e.Degree,
                        FieldOfStudy = e.FieldOfStudy,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        Description = e.Description
                    }).ToList(),
                Skills = p.Skills.OrderBy(s => s.Name).Select(s => s.Name).ToList(),
                Strength = score,
                Missing = missing
            };
        }
        public async Task<ProfileDto> GetMyAsync() => ToDto(await LoadAsync());

        public async Task<ProfileDto> UpdateBasicAsync(UpdateBasicDto dto)
        {
            var p = await LoadAsync();
            p.Headline = Clean(dto.Headline, 120);
            p.User.Location = Clean(dto.Location, 100);
            p.User.PhoneNumber = Clean(dto.Phone, 30);
            p.User.LinkedInUrl = CleanUrl(dto.LinkedInUrl);
            p.GithubUrl = CleanUrl(dto.GitHubUrl);
            p.WebsiteUrl = CleanUrl(dto.WebsiteUrl);
            p.IsOpenToWork = dto.IsOpenToWork;
            return await SaveAndReturnAsync(p);
        }
        public async Task<ProfileDto> UpdateAboutAsync(UpdateAboutDto dto)
        {
            var p = await LoadAsync();
            p.About = Clean(dto.About, 2000);
            return await SaveAndReturnAsync(p);
        }

        public async Task<ProfileDto> SaveExperienceAsync(int? id, Application.DTOs.JobSeeker.JobSeekers.ExperienceDto dto)
        {
            var title = Clean(dto.Title, 100); var company = Clean(dto.Company, 100);
            if (title == null || company == null) throw new ArgumentException("Title and company are required.");
            if (!dto.IsCurrent && dto.EndDate != null && dto.EndDate < dto.StartDate)
                throw new ArgumentException("End date can't be before the start date.");

            var p = await LoadAsync();
            Experience e;
            if (id == null) { e = new Experience { SeekerProfileId = p.Id }; p.Experiences.Add(e); }
            else e = p.Experiences.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Experience not found.");

            e.Title = title; e.Company = company;
            e.Location = Clean(dto.Location, 100);
            e.StartDate = dto.StartDate;
            e.IsCurrent = dto.IsCurrent;
            e.EndDate = dto.IsCurrent ? null : dto.EndDate;
            e.Description = Clean(dto.Description, 1500);
            return await SaveAndReturnAsync(p);
        }
        public async Task<ProfileDto> DeleteExperienceAsync(int id)
        {
            var p = await LoadAsync();
            var e = p.Experiences.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Experience not found.");
            p.Experiences.Remove(e);
            _db.Experiences.Remove(e);
            return await SaveAndReturnAsync(p);
        }
        public async Task<ProfileDto> SaveEducationAsync(int? id, Application.DTOs.JobSeeker.JobSeekers.EducationDto dto)
        {
            var school = Clean(dto.School, 120); var degree = Clean(dto.Degree, 120);
            if (school == null || degree == null) throw new ArgumentException("School and degree are required.");
            if (dto.EndDate != null && dto.EndDate < dto.StartDate)
                throw new ArgumentException("End date can't be before the start date.");

            var p = await LoadAsync();
            Education e;
            if (id == null) { e = new Education { SeekerProfileId = p.Id }; p.Educations.Add(e); }
            else e = p.Educations.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Education not found.");

            e.School = school; e.Degree = degree;
            e.FieldOfStudy = Clean(dto.FieldOfStudy, 120);
            e.StartDate = dto.StartDate; e.EndDate = dto.EndDate;
            e.Description = Clean(dto.Description, 1000);
            return await SaveAndReturnAsync(p);
        }

        public async Task<ProfileDto> DeleteEducationAsync(int id)
        {
            var p = await LoadAsync();
            var e = p.Educations.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Education not found.");
            p.Educations.Remove(e);
            _db.Educations.Remove(e);
            return await SaveAndReturnAsync(p);
        }

        public async Task<ProfileDto> SaveSkillsAsync(UpdateSkillsDto dto)
        {
            var names = dto.Skills.Select(s => Clean(s, 40)).Where(s => s != null)
                .Select(s => s!).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (names.Count > 30) throw new ArgumentException("You can add up to 30 skills.");

            var p = await LoadAsync();
            _db.Skills.RemoveRange(p.Skills);
            p.Skills.Clear();
            foreach (var n in names) p.Skills.Add(new Skill { SeekerProfileId = p.Id, Name = n });
            return await SaveAndReturnAsync(p);
        }
        public async Task<ProfileDto> SavePreferencesAsync(UpdatePreferencesDto dto)
        {
            if (dto.ExpectedSalary < 0) throw new ArgumentException("Salary can't be negative.");
            var p = await LoadAsync();
            p.DesiredRole = Clean(dto.DesiredRole, 100);
            p.PreferredJobType = Clean(dto.PreferredJobType, 50);
            p.ExpectedSalary = dto.ExpectedSalary;
            p.NoticePeriod = Clean(dto.NoticePeriod, 50);
            p.PreferredLocations = Clean(dto.PreferredLocations, 200);
            return await SaveAndReturnAsync(p);
        }

        public async Task<ProfileDto> UpdateProfileImge(UpdateProfileImage dto)
        {
            var file = dto.ProfileImage ?? throw new ArgumentException("Choose an image.");
            if (!file.ContentType.StartsWith("image/") || file.Length > 2 * 1024 * 1024)
                throw new ArgumentException("Photo must be an image under 2 MB.");

            var seeker = await _db.JobSeekers
                .Include(x => x.User)
                .Include(x => x.ProfileImage)
                .FirstOrDefaultAsync(x => x.UserId == _currentUser.UserId)
                ?? throw new KeyNotFoundException("Profile not found.");

            // 1. upload the new one first
            var (url, publicId) = await _cloudinaryService.UploadImageAsync(file);
            var oldPublicId = seeker.ProfileImage?.PublicId;

            if (seeker.ProfileImage == null)
            {
                seeker.ProfileImage = new ProfileImage
                {
                    ImageUrl = url,
                    PublicId = publicId,
                    CreatedAt = DateTime.UtcNow
                };
            }
            else
            {
                seeker.ProfileImage.ImageUrl = url;
                seeker.ProfileImage.PublicId = publicId;
                seeker.ProfileImage.CreatedAt = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldPublicId))
                await _cloudinaryService.DeleteImageAsync(oldPublicId);

            return ToDto(seeker);
        }

        public async Task<SuccessResponse> SaveVisibilityAsync(UpdateVisibilityDto dto)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException("Log in first.");
            var s = await _db.JobSeekers.FirstOrDefaultAsync(x => x.UserId == userId)
                    ?? throw new KeyNotFoundException("Profile not found.");

            s.IsPublic = dto.IsPublic;
            s.ShowEmail = dto.ShowEmail;
            s.ShowPhone = dto.ShowPhone;
            s.ShowCv = dto.ShowCv && (s.Resume != null || !string.IsNullOrEmpty(s.ResumeUrl));

            await _db.SaveChangesAsync();
            return new SuccessResponse { Message = "update visability" };
        }

        private IQueryable<JobSeeker> SeekerQuery() => _db.JobSeekers
    .Include(x => x.User)
    .Include(x => x.ProfileImage)
    .Include(x => x.Resume)
    .Include(x => x.Experiences)
    .Include(x => x.Educations)
    .Include(x => x.Skills)
    .AsSplitQuery();

        public async Task<PublicProfileDto> GetPublicAsync(int id)
        {
            var s = await SeekerQuery().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsPublic)
                ?? throw new KeyNotFoundException("Profile not found.");     // private and missing look the same

            var full = ToDto(s);
            return new PublicProfileDto
            {
                FullName = full.FullName,
                PhotoUrl = full.PhotoUrl,
                Headline = full.Headline,
                About = full.About,
                Location = full.Location,
                IsOpenToWork = full.IsOpenToWork,
                Email = s.ShowEmail ? full.Email : null,
                Phone = s.ShowPhone ? full.Phone : null,
                ResumeUrl = s.ShowCv ? full.ResumeUrl : null,
                LinkedInUrl = full.LinkedInUrl,
                GitHubUrl = full.GitHubUrl,
                WebsiteUrl = full.WebsiteUrl,
                Experiences = full.Experiences,
                Educations = full.Educations,
                Skills = full.Skills
            };
        }


    }
}