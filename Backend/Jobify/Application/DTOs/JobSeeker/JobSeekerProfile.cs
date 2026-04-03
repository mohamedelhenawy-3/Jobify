using Application.DTOs.Education;
using Application.DTOs.Experience;
using Application.DTOs.Skills;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker
{
    public class JobSeekerProfile
    {
        public bool IsPrivate { get; set; } = false;
        public string? PhoneNumber { get; set; }
        public string? ResumeUrl { get; set; }
        public string? Location { get; set; }
        public string? Message { get; set; }
        public ProfileImage? ProfileImage { get; set; }
        public List<MySkills> Skills { get; set; } = new();

        public List<ExperienceDto> Experiences { get; set; } = new();
        public List<EducationDto> Educations { get; set; } = new();
    }
}
