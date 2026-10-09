using Domain.Entities.LooksUp.EmployerPermisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class TeamMemberDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public EmployerCompanyRole Role { get; set; }
        public bool IsActive { get; set; }
        public List<string> Permissions { get; set; } = new();
        public int JobsPosted { get; set; }
        public DateTime? JoinedAt { get; set; }
        public bool IsMe { get; set; }
    }
}
