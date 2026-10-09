using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication.JobApplication
{
    public class RecentApplicantDto
    {
        public int Id { get; set; }
        public string ApplicantName { get; set; }
        public string? PhotoUrl { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }
    }
}
