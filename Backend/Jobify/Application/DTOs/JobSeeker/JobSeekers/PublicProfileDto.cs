using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers
{
    public class PublicProfileDto

    {
        public string FullName { get; set; } = "";
        public string? PhotoUrl { get; set; }
        public string? Headline { get; set; }
        public string? About { get; set; }
        public string? Location { get; set; }
        public bool IsOpenToWork { get; set; }
        public string? Email { get; set; }          // only if ShowEmail
        public string? Phone { get; set; }          // only if ShowPhone
        public string? ResumeUrl { get; set; }          // only if ShowCv
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public List<ExperienceDto> Experiences { get; set; } = new();
        public List<EducationDto> Educations { get; set; } = new();
        public List<string> Skills { get; set; } = new();
    }
}
