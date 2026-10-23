using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class CompanyMembersPreviewDto
    {
        public int Total { get; set; }
        public List<CompanyMemberDto> Items { get; set; } = new();
    }
}
