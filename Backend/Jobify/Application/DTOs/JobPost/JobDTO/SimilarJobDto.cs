using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class SimilarJobDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? CompanyName { get; set; }
        public string? Location { get; set; }
        public string JobType { get; set; } = "";
        public string ExperienceLevel { get; set; } = "";
        public decimal? MinSalary { get; set; }
        public decimal? MaxSalary { get; set; }
        public string? Logo { get; set; }
        public DateTime CreatedAt { get; set; }
        public int MatchPercent { get; set; }
    }
}
