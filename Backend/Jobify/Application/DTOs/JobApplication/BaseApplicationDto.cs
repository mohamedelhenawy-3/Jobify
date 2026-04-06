using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication
{
    public class BaseApplicationDto
    {
        public int ApplicationId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime AppliedAt { get; set; }
    }
}
