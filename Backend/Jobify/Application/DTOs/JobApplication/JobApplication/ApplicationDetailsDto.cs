using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication.JobApplication
{
    public class ApplicationDetailsDto
    {
        public int Id { get; set; }
        public ApplicationStatus Status { get; set; }
        public DateTime AppliedAt { get; set; }

        public int JobPostId { get; set; }
        public string JobTitle { get; set; } = "";

        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Phone { get; set; }
        public string? Logo { get; set; }

        public string? CoverLetter { get; set; }
        public string? ResumeUrl { get; set; }

        public InterviewDto? Interview { get; set; }
    }
}
