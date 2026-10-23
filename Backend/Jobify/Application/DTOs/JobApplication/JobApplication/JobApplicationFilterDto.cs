using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication.JobApplication
{
    public class JobApplicationFilterDto
    {
        public ApplicationStatus? Status { get; set; }
        public string? SearchTerm { get; set; }
    }
}
