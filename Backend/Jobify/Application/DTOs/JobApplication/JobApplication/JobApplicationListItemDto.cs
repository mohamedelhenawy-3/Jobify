using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication.JobApplication
{
    public class JobApplicationListItemDto
    {
        public int Id { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string logo { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }
        public ApplicationStatus Status { get; set; }
        public string StatusLabel => Status.ToString();
    }
}
