using Application.DTOs.Education;
using Application.DTOs.Experience;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.JobSeeker
{
    public class UpdateMyProfile
    {
        public string? FullName { get; set; }

        public string? Headline { get; set; }

        public string? Summary { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Location { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? GithubUrl { get; set; }

        public IFormFile? ProfileImage { get; set; }

        public List<string>? Skills { get; set; }

        // JSON STRINGS
        public string? EducationsJson { get; set; }

        public string? ExperiencesJson { get; set; }
    }
}