using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication.JobApplication
{
    public class HiringPipelineDto
    {
        public int New { get; set; }
        public int Applied { get; set; }
        public int UnderReview { get; set; }
        public int Shortlisted { get; set; }
        public int InterviewScheduled { get; set; }
        public int Interviewed { get; set; }
        public int Offered { get; set; }
        public int Hired { get; set; }
        public int Rejected { get; set; }
        public int Withdrawn { get; set; }
    }
}
