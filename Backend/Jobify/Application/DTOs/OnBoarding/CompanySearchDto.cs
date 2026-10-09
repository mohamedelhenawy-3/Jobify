using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OnBoarding
{
    public class CompanySearchDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public bool IsVerified { get; set; }
    }
}
