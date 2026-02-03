using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication
{
    public class MyApplications
    {

        public int ApplicationId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime AppliedAt { get; set; }

        public int JobId { get; set; }
        public string JobTitle { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string JobLocation { get; set; } = null!;
        public string WorkMode { get; set; } = null!;
        public string EmploymentType { get; set; } = null!;

        public string? CoverLetterUrl { get; set; }
    }
 }
