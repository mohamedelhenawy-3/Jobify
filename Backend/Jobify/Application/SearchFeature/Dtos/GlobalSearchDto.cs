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

        public List<int>? IndustryIds { get; set; }
        public List<int>? WorkModeIds { get; set; }
        public List<int>? EmploymentTypeIds { get; set; }
        public int? PostedWithinDays { get; set; }

        public string? Industry { get; set; }
        public string? CompanySize { get; set; }

        // Shared
        public string? Location { get; set; }
    }
}
