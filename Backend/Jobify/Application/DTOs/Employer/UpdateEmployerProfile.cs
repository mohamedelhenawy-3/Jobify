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
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Position { get; set; }
        public string? Bio { get; set; }

  
        public IFormFile? PImage { get; set; }
    }
}
