using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost
{
    public class JobDto
    {

        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        public decimal MinSalary { get; set; }
        public decimal MaxSalary { get; set; }
        public int WorkModeId { get; set; }
        public int IndustryId { get; set; }
        public int EmploymentTypeId { get; set; }
        public int StatusId { get; set; }
    }
}
