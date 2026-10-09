using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Industry { get; set; }
        public string? CompanySize { get; set; }
        public string? Founded { get; set; }

        public bool IsVerified { get; set; } = false;
        public CompanyVerificationStatus VerificationStatus { get; set; } = CompanyVerificationStatus.Pending;
        public string? RejectionReason { get; set; }
        public DateTime VerificationSubmittedAt { get; set; } = DateTime.UtcNow;

        public bool IsPublic { get; set; } = true;
        public int CreatedByEmployerId { get; set; }
        public EmployerProfile CreatedByEmployer { get; set; } = null!;

        public ICollection<EmployerProfile> EmployerProfiles { get; set; } = new List<EmployerProfile>(); public ProfileImage? Logo { get; set; }
        public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    }
}
