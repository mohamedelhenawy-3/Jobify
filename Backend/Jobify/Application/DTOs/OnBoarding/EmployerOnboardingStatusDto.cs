using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OnBoarding
{
    public class EmployerOnboardingStatusDto
    {
        public bool HasCompany { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        // Tracks Admin Approval ("Pending", "Approved", "Rejected")
        public string VerificationStatus { get; set; } = string.Empty;
        public bool IsVerified { get; set; }

        // Tracks Join Requests to existing companies
        public bool HasPendingJoinRequest { get; set; }
        public int? PendingCompanyId { get; set; }
    }
}
