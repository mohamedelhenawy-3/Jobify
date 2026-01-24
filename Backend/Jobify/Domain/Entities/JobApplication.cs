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

        public int? JobPostId { get; set; }
        public JobPost? JobPost { get; set; }

        public string? CoverLetterUrl { get; set; }
        public string? ResumeUrl { get; set; }

        public int StatusId { get; set; }
        public JobApplicationStatus Status { get; set; }

        public DateTime AppliedAt { get; set; }

        public ICollection<JobApplicationNote> ? Notes { get; set; }
    }
}
