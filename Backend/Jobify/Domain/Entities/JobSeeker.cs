using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class JobSeeker
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string? Headline { get; set; }
        public string? About { get; set; }

        public string? Summary { get; set; }  
        public string? GithubUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsOpenToWork { get; set; } = true;


        // job preferences (private)
        public string? DesiredRole { get; set; }
        public string? PreferredJobType { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? NoticePeriod { get; set; }
        public string? PreferredLocations { get; set; }

        // visibility
        public bool IsPublic { get; set; }
        public string? Username { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowCv { get; set; }

        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<SeekerLanguage> Languages { get; set; } = new List<SeekerLanguage>();
        public ICollection<SeekerCertificate> Certificates { get; set; } = new List<SeekerCertificate>();
        public ICollection<SeekerProject> Projects { get; set; } = new List<SeekerProject>();
        public ProfileImage? ProfileImage { get; set; }

        public string? ResumeUrl { get; set; }
        public Pdf? Resume { get; set; }
        public ICollection<JobApplication>? Applications { get; set; }
     


    }
}
