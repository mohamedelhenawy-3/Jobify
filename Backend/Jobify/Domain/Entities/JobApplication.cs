using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class JobApplication
    {
        public int Id { get; set; }

        public int JobSeekerId { get; set; }
        public JobSeeker? JobSeeker { get; set; }

        public int JobPostId { get; set; }
        public JobPost JobPost { get; set; }
        public string? CoverLetter { get; set; }
        public string? ExperienceSummary { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Motivation { get; set; }
        public string? ResumeUrl { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public ICollection<JobApplicationNote> ? Notes { get; set; }

    }
}
