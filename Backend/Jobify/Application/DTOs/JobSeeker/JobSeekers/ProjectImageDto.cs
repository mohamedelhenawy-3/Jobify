using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers
{
    public class ProjectImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = "";
        public bool IsCover { get; set; }
        public int DisplayOrder { get; set; }
    }
}
