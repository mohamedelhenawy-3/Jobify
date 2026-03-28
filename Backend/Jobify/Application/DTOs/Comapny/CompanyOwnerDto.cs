using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny
{
    public class CompanyOwnerDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Description { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? CompanySize { get; set; }
        public string? Founded { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // ===== PRIVATE STATS (owner only) =====
        public int TotalJobsPosted { get; set; }
        public int ActiveJobs { get; set; }
        public int DraftJobs { get; set; }
        public int ClosedJobs { get; set; }
        public int TotalApplicationsReceived { get; set; }
        public int PendingApplications { get; set; }
        public int AcceptedApplications { get; set; }
        public int RejectedApplications { get; set; }
        public int InterviewScheduled { get; set; }
    }
}
