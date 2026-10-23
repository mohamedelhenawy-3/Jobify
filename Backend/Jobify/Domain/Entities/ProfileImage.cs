using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ProfileImage
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }
        public string PublicId { get; set; } // for Cloudinary / Azure Blob (future)

        public DateTime CreatedAt { get; set; }

        public bool IsCover { get; set; }              // new (used by project gallery)
        public int DisplayOrder { get; set; }          // new (used by project gallery)
        public int? SeekerProjectId { get; set; }      // renamed from seekerprojectId
        public SeekerProject? SeekerProject { get; set; }

        public int? JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public int? EmployerProfileId { get; set; }
        public EmployerProfile EmployerProfile { get; set; }

    }
}
