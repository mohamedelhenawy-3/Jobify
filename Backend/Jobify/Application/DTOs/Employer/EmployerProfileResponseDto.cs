using Application.DTOs.Comapny;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Employer
{
    public class EmployerProfileResponseDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public string? LinkedInUrl { get; set; }

        public string? Position { get; set; }       
        public string? Bio { get; set; }            
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }

        public CompanyDto? Company { get; set; }
    }
}
