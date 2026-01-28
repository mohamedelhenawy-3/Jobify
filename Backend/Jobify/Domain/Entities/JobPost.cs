using Domain.Entities.LooksUp;
using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class JobPost
    {
        public int Id { get; set; }

        public int EmployerProfileId { get; set; }
        public EmployerProfile EmployerProfile { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }

        public int IndustryId { get; set; }
        public JobIndustry Industry { get; set; }

        public int EmploymentTypeId { get; set; }
        public EmploymentType EmploymentType { get; set; }

        public int StatusId { get; set; }
        public JobStatus Status { get; set; }

        public string CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; }
        public int WorkModeId { get; set; } = 1;
        public WorkMode WorkMode { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public ICollection<JobApplication> Applications { get; set; }
    }
}
