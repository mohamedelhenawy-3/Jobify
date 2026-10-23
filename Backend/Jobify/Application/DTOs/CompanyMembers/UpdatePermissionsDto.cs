using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class UpdatePermissionsDto
    {
        public List<string> Permissions { get; set; } = new();

    }
}
