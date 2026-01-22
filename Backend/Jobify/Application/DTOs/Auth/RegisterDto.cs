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
        public string Role { get; set; } = null!; // JobSeeker or Employer

        // JobSeeker
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }

        // Employer
        public string? CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
    }
}
