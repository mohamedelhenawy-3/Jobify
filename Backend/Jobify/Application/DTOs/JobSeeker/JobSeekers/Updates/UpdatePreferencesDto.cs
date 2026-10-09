using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers.Updates
{
    public class UpdatePreferencesDto
    {
        public string? DesiredRole { get; set; }
        public string? PreferredJobType { get; set; }
        public decimal? ExpectedSalary { get; set; }
        public string? NoticePeriod { get; set; }
        public string? PreferredLocations { get; set; }
    }
}
