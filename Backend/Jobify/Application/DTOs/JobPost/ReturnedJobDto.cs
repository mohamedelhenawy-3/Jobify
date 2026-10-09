using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost
{
    public class ReturnedJobDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string EmploymentType { get; set; } = string.Empty;
        public string ExprianceLevel { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Requirements { get; set; }
        public string? Responsibilities { get; set; }

        // Dates & Calculated Age
        public DateTime CreatedAt { get; set; }
        public string PostedAge { get; set; } = string.Empty; // e.g., "3 days ago", "2 weeks ago"

        // Company Metadata
        public string CompanyName { get; set; } = string.Empty;
        public string CompanyAge { get; set; } = string.Empty; // e.g., "5 years old"

        // Publisher & Engagement Details
        public int TotalApplications { get; set; }
        public string? PostedByEmployerId { get; set; }
        public string? PostedByName { get; set; }
        public bool IsOwnerView { get; set; }
    }
}
