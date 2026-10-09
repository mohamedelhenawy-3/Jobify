using Domain.Entities.LooksUp.JobEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class UpdateJobDto
    {

        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Requirements { get; set; }
        public string? Responsibilities { get; set; }

        public JobType? JobType { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public bool? IsActive { get; set; }


    }
}
