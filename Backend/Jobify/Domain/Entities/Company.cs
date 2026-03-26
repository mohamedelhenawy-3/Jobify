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
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relationship to Employer
        public int EmployerProfileId { get; set; }
        public EmployerProfile EmployerProfile { get; set; }
        public ProfileImage? Logo { get; set; }
        public ICollection<JobPost> JobPosts { get; set; } = new List<JobPost>();

    }
}
