using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication.JobApplication
{
    public class InterviewDto
    {
        public DateTime InterviewDate { get; set; }
        public string Location { get; set; } = "";
        public string? Notes { get; set; }
    }
}
