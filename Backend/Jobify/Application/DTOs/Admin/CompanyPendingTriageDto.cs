using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Admin
{
    public class CompanyPendingTriageDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string SubmittedByEmployerName { get; set; } = string.Empty;
        public string SubmittedByEmail { get; set; } = string.Empty;
        public DateTime VerificationSubmittedAt { get; set; }
    }
}
