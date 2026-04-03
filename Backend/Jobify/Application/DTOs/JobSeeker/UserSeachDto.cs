using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker
{
    public class UserSeachDto
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Location { get; set; }
        public string? ProfileImage { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
