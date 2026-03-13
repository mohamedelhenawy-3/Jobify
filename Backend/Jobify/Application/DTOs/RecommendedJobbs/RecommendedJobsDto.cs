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
        public string Title { get; set; } = null!;
        public string CompanyName { get; set; } = null!; 
        public int MatchScore { get; set; }        
        public string Location { get; set; } = null!;   
        public decimal MinSalary { get; set; }      
        public decimal MaxSalary { get; set; }           
        public string WorkMode { get; set; } = null!;   
    }
}
