using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.LooksUp.JobEnum
{
    public class JobCandidate
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Location { get; set; }
        public JobType JobType { get; set; }
        public ExperienceLevel Experience { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? Logo { get; set; }
        public string Requirements { get; set; } = "";
        public string Description { get; set; } = "";

        public HashSet<string> TitleWords { get; set; } = new();
        public HashSet<string> BodyWords { get; set; } = new();
    }
}
