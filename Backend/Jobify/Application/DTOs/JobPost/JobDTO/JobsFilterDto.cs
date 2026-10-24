using Domain.Entities.LooksUp.JobEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class JobsFilterDto
    {
        public string? Keyword { get; set; }
        public JobType? EmploymentType { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public string? Industry { get; set; }        
        public string? PostedWithin { get; set; }   
        public string? SortBy { get; set; } = "newest";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

    }
}
