using Domain.Entities.LooksUp.EmployerPermisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class MyTeamRoleDto
    {
        public EmployerCompanyRole Role { get; set; }
        public List<string> Permissions { get; set; } = new();
        public bool IsActive { get; set; }
        public bool HasAccess { get; set; }

    }
}
