using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User
{
    public class PublicUserProfileDto
    {
        public string Type { get; protected set; } = "";   // "employer" | "jobseeker"
        public int UserId { get; set; }
        public string FullName { get; set; } = "";
        public string? ImageUrl { get; set; }
        public bool IsPrivate { get; set; }
    }
}
