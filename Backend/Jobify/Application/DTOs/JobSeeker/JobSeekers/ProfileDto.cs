using Application.DTOs.Education;
using Application.DTOs.Experience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers
{
    public class ProfileDto
    {
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? PhotoUrl { get; set; }
        public string? Headline { get; set; }
        public string? About { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsOpenToWork { get; set; }

        public string? ResumeUrl { get; set; }
 

        public string? DesiredRole { get; set; }
        public string? PreferredJobType { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? NoticePeriod { get; set; }
        public string? PreferredLocations { get; set; }

        public bool IsPublic { get; set; }
        public string? Username { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowCv { get; set; }

        public List<ExperienceDto> Experiences { get; set; } = new();
        public List<EducationDto> Educations { get; set; } = new();
        public List<string> Skills { get; set; } = new();

        public int Strength { get; set; }
        public List<string> Missing { get; set; } = new();
    }
}
