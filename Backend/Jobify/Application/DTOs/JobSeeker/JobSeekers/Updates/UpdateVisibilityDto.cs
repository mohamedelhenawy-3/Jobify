using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers.Updates
{
    public class UpdateVisibilityDto
    {
        public bool IsPublic { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhone { get; set; }
        public bool ShowCv { get; set; }
    }
}
