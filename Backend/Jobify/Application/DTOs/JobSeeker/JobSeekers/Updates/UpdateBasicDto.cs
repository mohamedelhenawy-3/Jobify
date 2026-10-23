using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers.Updates
{
    public class UpdateBasicDto
    {
        public string? Headline { get; set; }
        public string? About { get; set; }
        public string? Location { get; set; }
        public string? Phone { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool IsOpenToWork { get; set; }
    }
}
