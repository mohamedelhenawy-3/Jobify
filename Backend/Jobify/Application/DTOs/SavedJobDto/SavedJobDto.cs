using Domain.Entities.LooksUp.JobEnum;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.SavedJobDto
{
    public class SavedJobDto
    {
        public int id { get; set; }
        public string? logo { get; set; }
        public string title { get; set; } = "";
        public string? location { get; set; }
        public JobType JobType { get; set; }               // keep your own types
        public ExperienceLevel ExperienceLevel { get; set; }
        public DateTime SavedAt { get; set; }
      
    }
}
