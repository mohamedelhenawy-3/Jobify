using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Description { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? CompanySize { get; set; }    // 1-10, 11-50, 51-200, 200+
        public string? Founded { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        // ===== STATS =====
        public int TotalJobs { get; set; }        
        public int ActiveJobs { get; set; }
    }
}
