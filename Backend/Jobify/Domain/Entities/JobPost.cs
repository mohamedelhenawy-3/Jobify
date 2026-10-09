using System;
using System.Collections.Generic;
using Domain.Entities.LooksUp.JobEnum;

namespace Domain.Entities
{
    public class JobPost
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        // e.g., FullTime, PartTime, Remote, Contract, Internship
        public JobType JobType { get; set; } = JobType.FullTime;

        // e.g., EntryLevel, MidLevel, Senior, Lead
        public ExperienceLevel ExperienceLevel { get; set; } = ExperienceLevel.MidLevel;

  
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string Currency { get; set; } = "USD";

        public string Requirements { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;

 
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(30);

  
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int PostedByEmployerId { get; set; }
        public EmployerProfile PostedByEmployer { get; set; } = null!;
        public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    }
}