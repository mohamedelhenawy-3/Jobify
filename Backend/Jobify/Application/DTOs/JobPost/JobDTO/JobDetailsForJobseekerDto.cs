using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class JobDetailsForJobseekerDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public string Requirements { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public string PostedAge { get; set; } = string.Empty;

        // Company Details
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? CompanyLogo { get; set; }
        public string CompanyWebsite { get; set; } = string.Empty;

        // Jobseeker Application Status Flags
        public bool HasApplied { get; set; }
        public int? ApplicationId { get; set; }
        public string? ApplicationStatus { get; set; }
        public bool IsActive { get; set; }
    }
}
