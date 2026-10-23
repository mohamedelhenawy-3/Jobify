using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny.RefactorCompany
{
    public class PublicCompanyProfileDto
    {
        public string Type { get; set; } = "company";
        public int Id { get; set; }
        public string CompanyName { get; set; } = "";
        public string? Logo { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public string? CompanySize { get; set; }
        public string? Founded { get; set; }
        public string? CompanyWebsite { get; set; }
        public bool IsVerified { get; set; }

        public int MembersCount { get; set; }
        public int OpenJobsCount { get; set; }
        public List<PublicCompanyJobDto> OpenJobs { get; set; } = new();
    }
}
