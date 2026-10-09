using Application.DTOs.Comapny;
using Application.DTOs.Education;
using Application.DTOs.Experience;
using Application.DTOs.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class GetUserProfile
    {
        public string Id { get; set; } = null!;

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? ProfileImage { get; set; }
        public string? Resume { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? ResumeUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GithubUrl { get; set; }
        public bool isPrivate { get; set; }
        public List<MySkills> Skills { get; set; } = new();

        public List<ExperienceDto> Experiences { get; set; } = new();
        public List<EducationDto> Educations { get; set; } = new();
   
        public string? Bio { get; set; }      
        public string? Position { get; set; }  
   
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
  
        public string UserRole { get; set; } = null!; 

        public CompanyDto? Company { get; set; }
    }
}
