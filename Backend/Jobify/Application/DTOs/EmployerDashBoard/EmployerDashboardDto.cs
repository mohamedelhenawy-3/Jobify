using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.EmployerDashBoard
{
    public class EmployerDashboardDto
    {
        public int ActiveJobs { get; set; }
        public int ExpiringSoon { get; set; }
        public int TotalApplications { get; set; }
        public int NewToday { get; set; }

        public List<EmployerJobDto> PostedJobs { get; set; }
        public List<RecentApplicationDto> RecentApplications { get; set; }
    }
}
