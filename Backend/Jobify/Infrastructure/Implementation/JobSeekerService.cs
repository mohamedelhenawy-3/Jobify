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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation
{
    public class JobSeekerService : IJobSeekerService
    {
        private static readonly string[] LanguageLevels = { "Basic", "Conversational", "Fluent", "Native" };

        private const int MaxImagesPerProject = 10;
        private const long MaxProjectImageBytes = 5 * 1024 * 1024;

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
            SupabaseService supabaseservice)
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

            return new ResumeDto
            {
                PdfUrl = pdf.PdfUrl
            };
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

            await _supabaseservice.DeleteResume(oldUrl);

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
                .Include(x => x.User)
                .Include(x => x.ProfileImage)
                .Include(x => x.Resume)
                .Include(x => x.Experiences)
                .Include(x => x.Educations)
                .Include(x => x.Skills)
                .Include(x => x.Languages)
                .Include(x => x.Certificates)
                .Include(x => x.Projects)
                    .ThenInclude(pr => pr.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (p == null)
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
            Check(!string.IsNullOrEmpty(p.Headline), 5, "Add a headline");
            Check(!string.IsNullOrEmpty(p.About), 10, "Write a short about section");
            Check(!string.IsNullOrEmpty(p.User?.Location), 5, "Add your location");
            Check(p.Experiences.Count > 0, 15, "Add your work experience");
            Check(p.Educations.Count > 0, 10, "Add your education");
            Check(p.Skills.Count >= 5, 10, "Add at least 5 skills");
            Check(!string.IsNullOrEmpty(p.Resume?.PdfUrl), 10, "Upload your CV");
            Check(!string.IsNullOrEmpty(p.DesiredRole), 5, "Set your job preferences");
            Check(p.Languages.Count > 0, 5, "Add your languages");
            Check(p.Certificates.Count > 0, 5, "Add a certificate");
            Check(p.Projects.Count > 0, 10, "Add a project");

            return new ProfileDto
            {
                FullName = p.User?.FullName,
                Email = p.User?.Email ?? "",
                PhotoUrl = p.ProfileImage?.ImageUrl,
                Headline = p.Headline,
                About = p.About,
                ResumeUrl = p.Resume?.PdfUrl,
                Location = p.User?.Location,
                Phone = p.User?.PhoneNumber,
                LinkedInUrl = p.User?.LinkedInUrl,
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
                Languages = p.Languages.OrderBy(l => l.Name)
                    .Select(l => new LanguageDto
                    {
                        Id = l.Id,
                        Name = l.Name,
                        Level = l.Level
                    }).ToList(),
                Certificates = p.Certificates.OrderByDescending(c => c.IssueDate)
                    .Select(c => new CertificateDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Issuer = c.Issuer,
                        IssueDate = c.IssueDate,
                        ExpiryDate = c.ExpiryDate,
                        CredentialUrl = c.CredentialUrl
                    }).ToList(),

                // CHANGED: gallery mapping (no dates, with images)
                Projects = p.Projects.OrderBy(x => x.DisplayOrder).ThenByDescending(x => x.CreatedAt)
                    .Select(x =>
                    {
                        var images = x.Images.OrderBy(i => i.DisplayOrder)
                            .Select(i => new ProjectImageDto
                            {
                                Id = i.Id,
                                ImageUrl = i.ImageUrl,
                                IsCover = i.IsCover,
                                DisplayOrder = i.DisplayOrder
                            }).ToList();

                        return new ProjectDto
                        {
                            Id = x.Id,
                            Title = x.Title,
                            Description = x.Description,
                            Url = x.Url,
                            Technologies = x.Technologies,
                            DisplayOrder = x.DisplayOrder,
                            Images = images,
                            CoverImageUrl = (images.FirstOrDefault(i => i.IsCover) ?? images.FirstOrDefault())?.ImageUrl
                        };
                    }).ToList(),
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
            var title = Clean(dto.Title, 100);
            var company = Clean(dto.Company, 100);
            if (title == null || company == null) throw new ArgumentException("Title and company are required.");
            if (!dto.IsCurrent && dto.EndDate != null && dto.EndDate < dto.StartDate)
                throw new ArgumentException("End date can't be before the start date.");

            var p = await LoadAsync();
            Experience e;
            if (id == null) { e = new Experience { SeekerProfileId = p.Id }; p.Experiences.Add(e); }
            else e = p.Experiences.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Experience not found.");

            e.Title = title;
            e.Company = company;
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
            var school = Clean(dto.School, 120);
            var degree = Clean(dto.Degree, 120);
            if (school == null || degree == null) throw new ArgumentException("School and degree are required.");
            if (dto.EndDate != null && dto.EndDate < dto.StartDate)
                throw new ArgumentException("End date can't be before the start date.");

            var p = await LoadAsync();
            Education e;
            if (id == null) { e = new Education { SeekerProfileId = p.Id }; p.Educations.Add(e); }
            else e = p.Educations.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Education not found.");

            e.School = school;
            e.Degree = degree;
            e.FieldOfStudy = Clean(dto.FieldOfStudy, 120);
            e.StartDate = dto.StartDate;
            e.EndDate = dto.EndDate;
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

        public async Task<ProfileDto> SaveLanguageAsync(int? id, LanguageDto dto)
        {
            var name = Clean(dto.Name, 50);
            if (name == null) throw new ArgumentException("Language is required.");
            if (!LanguageLevels.Contains(dto.Level)) throw new ArgumentException("Invalid level.");

            var p = await LoadAsync();
            if (p.Languages.Any(l => l.Id != id && l.Name.ToLower() == name.ToLower()))
                throw new InvalidOperationException("You already added this language.");

            SeekerLanguage l;
            if (id == null) { l = new SeekerLanguage { JobSeekerProfileId = p.Id }; p.Languages.Add(l); }
            else l = p.Languages.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Language not found.");

            l.Name = name;
            l.Level = dto.Level;
            return await SaveAndReturnAsync(p);
        }

        public async Task<ProfileDto> DeleteLanguageAsync(int id)
        {
            var p = await LoadAsync();
            var l = p.Languages.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Language not found.");
            p.Languages.Remove(l);
            _db.Languages.Remove(l);
            return await SaveAndReturnAsync(p);
        }

        public async Task<ProfileDto> SaveCertificateAsync(int? id, CertificateDto dto)
        {
            var name = Clean(dto.Name, 150);
            var issuer = Clean(dto.Issuer, 150);
            if (name == null || issuer == null) throw new ArgumentException("Name and issuer are required.");
            if (dto.ExpiryDate != null && dto.ExpiryDate < dto.IssueDate)
                throw new ArgumentException("Expiry date can't be before the issue date.");

            var url = CleanUrl(dto.CredentialUrl);

            var p = await LoadAsync();
            SeekerCertificate c;
            if (id == null) { c = new SeekerCertificate { JobSeekerProfileId = p.Id }; p.Certificates.Add(c); }
            else c = p.Certificates.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Certificate not found.");

            c.Name = name;
            c.Issuer = issuer;
            c.IssueDate = dto.IssueDate;
            c.ExpiryDate = dto.ExpiryDate;
            c.CredentialUrl = url;
            return await SaveAndReturnAsync(p);
        }

        public async Task<ProfileDto> DeleteCertificateAsync(int id)
        {
            var p = await LoadAsync();
            var c = p.Certificates.FirstOrDefault(x => x.Id == id) ?? throw new KeyNotFoundException("Certificate not found.");
            p.Certificates.Remove(c);
            _db.Certificates.Remove(c);
            return await SaveAndReturnAsync(p);
        }

        // =================== PROJECTS / GALLERY ===================

        // CHANGED: dates removed, DisplayOrder added
        public async Task<ProfileDto> SaveProjectAsync(int? id, ProjectDto dto)
        {
            var title = Clean(dto.Title, 120);
            if (title == null) throw new ArgumentException("Title is required.");

            var url = CleanUrl(dto.Url);

            var p = await LoadAsync();
            SeekerProject x;
            if (id == null)
            {
                x = new SeekerProject
                {
                    JobSeekerProfileId = p.Id,
                    DisplayOrder = p.Projects.Count == 0 ? 0 : p.Projects.Max(i => i.DisplayOrder) + 1
                };
                p.Projects.Add(x);
            }
            else
            {
                x = p.Projects.FirstOrDefault(i => i.Id == id)
                    ?? throw new KeyNotFoundException("Project not found.");
            }

            x.Title = title;
            x.Description = Clean(dto.Description, 1500);
            x.Url = url;
            x.Technologies = Clean(dto.Technologies, 200);
            return await SaveAndReturnAsync(p);
        }

        // CHANGED: also deletes the project's images from Cloudinary
        public async Task<ProfileDto> DeleteProjectAsync(int id)
        {
            var p = await LoadAsync();
            var x = p.Projects.FirstOrDefault(i => i.Id == id)
                ?? throw new KeyNotFoundException("Project not found.");

            var publicIds = x.Images.Select(i => i.PublicId).ToList();

            p.Projects.Remove(x);
            _db.Projects.Remove(x);                  // image rows cascade in the DB
            var result = await SaveAndReturnAsync(p);

            foreach (var pid in publicIds)
                if (!string.IsNullOrEmpty(pid))
                    await _cloudinaryService.DeleteImageAsync(pid);

            return result;
        }

        // NEW
        public async Task<ProfileDto> AddProjectImagesAsync(int projectId, List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                throw new ArgumentException("Choose at least one image.");

            foreach (var f in files)
                if (!f.ContentType.StartsWith("image/") || f.Length == 0 || f.Length > MaxProjectImageBytes)
                    throw new ArgumentException("Each file must be an image up to 5 MB.");

            var p = await LoadAsync();
            var x = p.Projects.FirstOrDefault(i => i.Id == projectId)
                ?? throw new KeyNotFoundException("Project not found.");

            if (x.Images.Count + files.Count > MaxImagesPerProject)
                throw new ArgumentException($"A project can have up to {MaxImagesPerProject} images.");

            var uploaded = new List<string>();
            try
            {
                var order = x.Images.Count == 0 ? 0 : x.Images.Max(i => i.DisplayOrder) + 1;
                var hasCover = x.Images.Any(i => i.IsCover);

                foreach (var file in files)
                {
                    var (url, publicId) = await _cloudinaryService.UploadImageAsync(file);
                    uploaded.Add(publicId);

                    x.Images.Add(new ProfileImage
                    {
                        ImageUrl = url,
                        PublicId = publicId,
                        CreatedAt = DateTime.UtcNow,
                        DisplayOrder = order++,
                        IsCover = !hasCover          // first image becomes the cover
                    });
                    hasCover = true;
                }

                return await SaveAndReturnAsync(p);
            }
            catch
            {
                // don't leave orphan files in Cloudinary if the save failed
                foreach (var pid in uploaded) await _cloudinaryService.DeleteImageAsync(pid);
                throw;
            }
        }

        // NEW
        public async Task<ProfileDto> DeleteProjectImageAsync(int projectId, int imageId)
        {
            var p = await LoadAsync();
            var x = p.Projects.FirstOrDefault(i => i.Id == projectId)
                ?? throw new KeyNotFoundException("Project not found.");
            var img = x.Images.FirstOrDefault(i => i.Id == imageId)
                ?? throw new KeyNotFoundException("Image not found.");

            x.Images.Remove(img);
            _db.Set<ProfileImage>().Remove(img);     // removing from the collection alone would only null the FK

            if (img.IsCover)
            {
                var next = x.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault();
                if (next != null) next.IsCover = true;
            }

            var result = await SaveAndReturnAsync(p);
            if (!string.IsNullOrEmpty(img.PublicId))
                await _cloudinaryService.DeleteImageAsync(img.PublicId);
            return result;
        }

        // NEW
        public async Task<ProfileDto> SetProjectCoverAsync(int projectId, int imageId)
        {
            var p = await LoadAsync();
            var x = p.Projects.FirstOrDefault(i => i.Id == projectId)
                ?? throw new KeyNotFoundException("Project not found.");
            if (!x.Images.Any(i => i.Id == imageId))
                throw new KeyNotFoundException("Image not found.");

            foreach (var i in x.Images) i.IsCover = i.Id == imageId;
            return await SaveAndReturnAsync(p);
        }

        // =================== PROFILE PHOTO / VISIBILITY / PUBLIC ===================

        public async Task<ProfileDto> UpdateProfileImge(UpdateProfileImage dto)
        {
            var file = dto.ProfileImage ?? throw new ArgumentException("Choose an image.");
            if (!file.ContentType.StartsWith("image/") || file.Length > 2 * 1024 * 1024)
                throw new ArgumentException("Photo must be an image under 2 MB.");

            var seeker = await _db.JobSeekers
                .Include(x => x.ProfileImage)
                .FirstOrDefaultAsync(x => x.UserId == _currentUser.UserId)
                ?? throw new KeyNotFoundException("Profile not found.");

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

            return ToDto(await LoadAsync());
        }

        public async Task<SuccessResponse> SaveVisibilityAsync(UpdateVisibilityDto dto)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException("Log in first.");
            var s = await _db.JobSeekers
                .Include(x => x.Resume)
                .FirstOrDefaultAsync(x => x.UserId == userId)
                ?? throw new KeyNotFoundException("Profile not found.");

            s.IsPublic = dto.IsPublic;

            await _db.SaveChangesAsync();
            return new SuccessResponse { Message = "update visability" };
        }

        // CHANGED: Projects now include their Images (public profile shows the gallery)
        private IQueryable<JobSeeker> SeekerQuery() => _db.JobSeekers
            .Include(x => x.User)
            .Include(x => x.ProfileImage)
            .Include(x => x.Resume)
            .Include(x => x.Experiences)
            .Include(x => x.Educations)
            .Include(x => x.Skills)
            .Include(x => x.Languages)
            .Include(x => x.Certificates)
            .Include(x => x.Projects)
                .ThenInclude(pr => pr.Images)
            .AsSplitQuery();

        public async Task<PublicProfileDto> GetPublicAsync(int id)
        {
            var s = await SeekerQuery().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.IsPublic)
                ?? throw new KeyNotFoundException("Profile not found.");

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
                Skills = full.Skills,
                Languages = full.Languages,
                Certificates = full.Certificates,
                Projects = full.Projects
            };
        }
    }
}