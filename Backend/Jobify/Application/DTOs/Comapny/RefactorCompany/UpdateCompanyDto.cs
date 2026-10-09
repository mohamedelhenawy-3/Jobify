using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny.RefactorCompany
{
    public class UpdateCompanyDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public string? CompanySize { get; set; }
        public string? Founded { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
