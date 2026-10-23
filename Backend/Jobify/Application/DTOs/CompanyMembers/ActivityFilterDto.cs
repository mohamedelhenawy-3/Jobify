using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class ActivityFilterDto
    {
        public string? Type { get; set; }
        public string? Category { get; set; }          // Team | Company | Jobs | Applicants | Profile
        public int? ActorEmployerId { get; set; }      // company feed only
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? Search { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
