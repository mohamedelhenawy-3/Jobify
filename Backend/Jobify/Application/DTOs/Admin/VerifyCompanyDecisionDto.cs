using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Admin
{
    public class VerifyCompanyDecisionDto
    {
        public bool IsApproved { get; set; }
        public string? RejectionReason { get; set; }
    }
}
