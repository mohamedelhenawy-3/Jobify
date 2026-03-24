using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication
{
    public class AppliedJobSeekers
    {
        public int ApplicationId { get; set; }

        public string? ResumeUrl { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string JobTitle { get; set; }
        public string JoSeekerEmail { get; set; }
        public string JoSeekerFullName { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }


    }
}
