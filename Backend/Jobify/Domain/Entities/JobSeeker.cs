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
        public string? Summary { get; set; }
   
        public string? ResumeUrl { get; set; }
     
        public string? GithubUrl { get; set; }
        public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
        public ICollection<Education> Educations { get; set; } = new List<Education>();
        public ProfileImage? ProfileImage { get; set; }

         public Pdf? Resume { get; set; }
        public ICollection<JobApplication>? Applications { get; set; }
        public ICollection<Skill> Skills { get; set; } = new List<Skill>();


    }
}
