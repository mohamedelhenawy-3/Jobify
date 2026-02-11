using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.EmployerDashBoard
{
    public class RecentApplicationDto
    {
        public string ApplicantName { get; set; }
        public string JobTitle { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }

        public string? ProfileImageUrl { get; set; }   // 👈 ADD THIS
    }
}
