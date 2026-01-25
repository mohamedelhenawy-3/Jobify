using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class JobSeeker
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ResumeUrl { get; set; }
        public string? Location { get; set; }
        public ProfileImage? ProfileImage { get; set; }

         public Pdf? Resume { get; set; }
        public ICollection<JobApplication>? Applications { get; set; }


    }
}
