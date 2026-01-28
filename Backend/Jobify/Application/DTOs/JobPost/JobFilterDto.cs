using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost
{
    public class JobFilterDto
    {
        public int? IndustryId { get; set; }         // Filter by industry (optional)
        public int? EmploymentTypeId { get; set; }   // Filter by employment type (optional)
        public int? WorkModeId { get; set; }         // Filter by work mode (optional)
        public int? PostedWithinDays { get; set; }   // Filter jobs posted in the last X days
        public string? Keyword { get; set; } //jobName
    }
}
