using Domain.Entities.LooksUp.EmployerPermisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class ChangeMemberRoleDto
    {
        public EmployerCompanyRole NewRole { get; set; }
    }
}
