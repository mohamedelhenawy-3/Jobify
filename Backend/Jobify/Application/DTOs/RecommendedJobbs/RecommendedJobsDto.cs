using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.RecommendedJobbs
{
    public class RecommendedJobsDto
    {
        public int JobId { get; set; }    
        public string logo { get; set; }
        public string Title { get; set; } = null!;
        public string CompanyName { get; set; } = null!; 
        public int MatchScore { get; set; }        
        public string Location { get; set; } = null!;   
        public decimal? MinSalary { get; set; }      
        public decimal? MaxSalary { get; set; }
        public int WorkModeId { get; set; }
        public int IndustryId { get; set; }
        public int EmploymentTypeId { get; set; }
        public int StatusId { get; set; }
    }
}
