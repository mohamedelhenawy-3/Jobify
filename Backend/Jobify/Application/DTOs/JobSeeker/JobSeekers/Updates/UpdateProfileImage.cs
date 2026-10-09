using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers.Updates
{
    public class UpdateProfileImage
    {
        public IFormFile? ProfileImage { get; set; }
    }
}
