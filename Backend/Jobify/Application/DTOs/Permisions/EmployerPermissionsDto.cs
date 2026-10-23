using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Permisions
{
    public class EmployerPermissionsDto
    {
        public int EmployerId { get; set; }
        public string Role { get; set; } = default!;
        public List<string> Permissions { get; set; } = new();
    }
}
