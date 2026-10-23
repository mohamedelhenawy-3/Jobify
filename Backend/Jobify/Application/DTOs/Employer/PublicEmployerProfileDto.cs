using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Employer
{
    public class PublicEmployerProfileDto : PublicUserProfileDto
    {
        public PublicEmployerProfileDto() { Type = "enployer"; }
        public int EmployerId { get; set; }
        public string? Bio { get; set; }
        public string? Position { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Role { get; set; }
        public DateTime? JoinedCompanyAt { get; set; }

        public int JobsPosted { get; set; }
        public int OpenJobsCount { get; set; }
        public List<PublicJobSummaryDto> RecentJobs { get; set; } = new();

        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
    }
}
