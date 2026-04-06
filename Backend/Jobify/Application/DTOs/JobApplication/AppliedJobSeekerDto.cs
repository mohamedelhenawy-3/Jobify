using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication
{
    public class AppliedJobSeekerDto :BaseApplicationDto
    {
        public string? ResumeUrl { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string JobTitle { get; set; } = null!;
        public string JobSeekerEmail { get; set; } = null!;
        public string JobSeekerFullName { get; set; } = null!;
        public string? JobSeekerHeadline { get; set; }
    }
}
