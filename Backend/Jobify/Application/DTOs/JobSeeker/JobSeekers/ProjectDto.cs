using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers
{
    public class ProjectDto
    {
        public int? Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Technologies { get; set; }

        public int DisplayOrder { get; set; }
        public string? CoverImageUrl { get; set; }
        public List<ProjectImageDto> Images { get; set; } = new();
    }
}
