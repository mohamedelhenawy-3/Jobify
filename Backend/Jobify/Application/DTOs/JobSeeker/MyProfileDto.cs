using Application.DTOs.Skills;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker
{
    public class MyProfileDto
    {
        public string? PhoneNumber { get; set; }
        public string? ResumeUrl { get; set; }
        public string? Location { get; set; }
        public string? ProfileImage { get; set; }
        public string? Headline { get; set; }
        public string? Summary { get; set; }
        public List<MySkills> Skills { get; set; } = new();



    }
}
