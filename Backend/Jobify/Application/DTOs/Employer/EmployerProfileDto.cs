using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Employer
{
    public class EmployerProfileDto
    {
        public string FullName { get; set; } = default!;
  
        public string? Bio { get; set; }
        public string Email { get; set; } = default!;
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string?  LinkedIn { get; set; }
        public string? PhotoUrl { get; set; }
        public string CompanyName { get; set; } = default!;
        public string? CompanyIndustry { get; set; }
        public string? CompanyWebsite { get; set; }

        public int JobsPostedCount { get; set; }
        public int TotalApplicants { get; set; }
        public int ActiveJobsCount { get; set; }

        public string Role { get; set; } = default!;
        public List<string> Permissions { get; set; } = new();
    }
}
