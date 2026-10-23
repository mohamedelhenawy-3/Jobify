using Application.DTOs.User;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers
{
    public class PublicJobSeekerProfileDto : PublicUserProfileDto
    {
        public PublicJobSeekerProfileDto() { Type = "jobseeker"; }

        public int SeekerId { get; set; }
        public string? Headline { get; set; }
        public string? Location { get; set; }
        public string? About { get; set; }
        public string? Summary { get; set; }
        public string? GithubUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsOpenToWork { get; set; } = true;

        public List<ExperienceDto> Experiences { get; set; } = new List<ExperienceDto>();
        public List<EducationDto> Educations { get; set; } = new List<EducationDto>();
        public List<string> Skills { get; set; } = new List<string>();

    }
}
