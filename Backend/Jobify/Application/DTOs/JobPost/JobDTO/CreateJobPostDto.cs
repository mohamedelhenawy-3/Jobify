using Domain.Entities.LooksUp.JobEnum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class CreateJobPostDto
    {
        [Required(ErrorMessage = "Job title is required.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Job description is required.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; } = string.Empty;

        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }

        public string Currency { get; set; } = "USD";

        public string? Requirements { get; set; }
        public string? Responsibilities { get; set; }

        [Required(ErrorMessage = "Job type is required.")]
        public JobType JobType { get; set; }

        [Required(ErrorMessage = "Experience level is required.")]
        public ExperienceLevel ExperienceLevel { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? ExpirationDate { get; set; }


    }
}
