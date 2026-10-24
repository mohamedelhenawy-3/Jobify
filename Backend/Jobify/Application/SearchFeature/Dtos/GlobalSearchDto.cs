using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SearchFeature.Dtos
{
    public class GlobalSearchDto
    {
        public string? Keyword { get; set; }
        public string Type { get; set; } = "all";
        public string? EmploymentType { get; set; }   // matches your JobType enum as string
        public string? ExperienceLevel { get; set; }

        public int? PostedWithinDays { get; set; }

        public int? IndustryId { get; set; }      // ✅ the new dynamic filter
        public string? CompanySize { get; set; }

        // Shared
        public string? Location { get; set; }
    }
}
