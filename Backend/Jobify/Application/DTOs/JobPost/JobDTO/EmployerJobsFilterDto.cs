using Domain.Entities.LooksUp.JobEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO
{
    public class EmployerJobsFilterDto : JobsFilterDto
    {
      
        public JobStatus? Status { get; set; }       
     
    }
}
