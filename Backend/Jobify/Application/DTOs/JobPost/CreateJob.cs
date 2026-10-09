using Domain.Entities;
using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost
{
    public class CreateJob
    {


        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }

        public string Requirements { get; set; } = string.Empty;
        public string Responsibilities { get; set; } = string.Empty;

        public DateTime? ExpirationDate { get; set; }
        public int IndustryId { get; set; }
        public int WorkModeId { get; set; }
        public int EmploymentTypeId { get; set; }
        public int StatusId { get; set; }
    }
}
