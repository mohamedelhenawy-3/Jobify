using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class EmployerJobItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Location { get; set; }
        public string JobType { get; set; }
        public string ExperienceLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public int ApplicantsCount { get; set; }
        public string? PostedBy { get; set; }

    }
}
