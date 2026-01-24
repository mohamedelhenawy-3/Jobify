using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Employer
{
    public class UpdateEmployerProfile
    {
        public string? CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public IFormFile? ProfileImage { get; set; } // optional
    }
}
