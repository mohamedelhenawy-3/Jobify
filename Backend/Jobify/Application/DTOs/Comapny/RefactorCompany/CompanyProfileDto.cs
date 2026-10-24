using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny.RefactorCompany
{
    public class CompanyProfileDto
    {
        public int Id { get; set; }
        public string Logo { get; set; }

        public string CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public int? IndustryId { get; set; }     // ✅ add (to preselect the dropdown)
        public string? CompanySize { get; set; }
        public string? Founded { get; set; }

        public List<string> Permissions { get; set; } = new();
        public bool IsVerified { get; set; } = false;
        public string MyRole { get; set; }
        public bool CanEdit { get; set; }
    }
}
