using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class RemoveMemberDto
    {
        public int? ReassignToEmployerId { get; set; }   // optional: who takes over the jobs

    }
}
