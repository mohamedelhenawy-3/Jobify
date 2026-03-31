using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class RegisterDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }
        public string Role { get; set; } = null!; // JobSeeker or Employer

        // JobSeeker
  
        public string? PhoneNumber { get; set; }
        public string? Position { get; set; }

        public string ConfirmPassword { get; set; }
        public string location { get; set; }
     
        public string? Headline { get; set; }
        public string? CompanyLocation { get; set; }
        //employer
        public string? CompanyName { get; set; }  // Required to register
        public string? CompanyWebsite { get; set; }
        public string? Industry { get; set; }
    }
}
