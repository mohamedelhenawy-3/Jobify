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
        public string Title { get; set; }
        public string Description { get; set; }
        public string Responsibilities { get; set; }
        public string EducationOrExperience { get; set; }
        public string Requirements { get; set; }
        public string Location { get; set; }
        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }

        public string WorkMode { get; set; }
        public string Industry { get; set; }
        public string EmploymentType { get; set; }
        public string Status { get; set; }
    }
}
