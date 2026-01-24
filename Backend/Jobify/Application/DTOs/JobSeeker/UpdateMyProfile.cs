using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker
{
    public class UpdateMyProfile
    {
        public string? PhoneNumber { get; set; }
        public string? ResumeUrl { get; set; }
        public string? Location { get; set; }
        public ProfileImage? ProfileImage { get; set; }

    }
}
