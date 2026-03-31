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
        public string logo { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public string location { get; set; }
        public int WorkModeId { get; set; }
        public int IndustryId { get; set; }
        public int EmploymentTypeId { get; set; }
        public int StatusId { get; set; }
        public int countJobs { get; set; }
    }
}
