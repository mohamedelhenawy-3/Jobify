using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class CompanyMemberDto
    {
        public string logo { get; set; }
        public int EmployerId { get; set; }
        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public List<string> Permissions { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime? JoinedCompanyAt { get; set; }
    }
}
