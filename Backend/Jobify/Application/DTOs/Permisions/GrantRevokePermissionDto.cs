using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Permisions
{
    public class GrantRevokePermissionDto
    {
        public int TargetEmployerId { get; set; }
        public string Permission { get; set; } = default!;
    }
}
