using Domain.Entities.LooksUp.JobEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class EmployerJobCardDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Logo { get; set; }
        public string Location { get; set; } = string.Empty;

        public JobType EmploymentType { get; set; }
        public ExperienceLevel ExperienceLevel { get; set; }
        public JobStatus Status { get; set; }

        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string Currency { get; set; } = "USD";

        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string PostedAge { get; set; } = string.Empty;

        public int TotalApplications { get; set; }
        public string PostedByName { get; set; } = string.Empty;
        public bool IsOwnerView { get; set; }
    }
}
