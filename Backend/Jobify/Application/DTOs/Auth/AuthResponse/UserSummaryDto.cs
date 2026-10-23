using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth.AuthResponse
{
    public class UserSummaryDto
    {

        public string FullName { get; set; } = "";
        public string? PhotoUrl { get; set; }
    }
}
