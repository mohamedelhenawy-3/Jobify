using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker
{
    public class UpdateMyProfile
    {
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Location { get; set; }
        public IFormFile? ProfileImage { get; set; }

    }
}
