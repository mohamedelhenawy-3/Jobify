using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost
{
    public class JobFilterDto
    {
        public string? Keyword { get; set; }
        public List<int>? IndustryIds { get; set; }        // multiple industries
        public List<int>? EmploymentTypeIds { get; set; } // multiple types
        public List<int>? WorkModeIds { get; set; }       // multiple modes
        public int? PostedWithinDays { get; set; }
    }
}
