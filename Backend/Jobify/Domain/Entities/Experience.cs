using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Experience
    {
        public int Id { get; set; }

        public string CompanyName { get; set; } = null!;
        public string JobTitle { get; set; } = null!;
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; } = null!;
    }
}
