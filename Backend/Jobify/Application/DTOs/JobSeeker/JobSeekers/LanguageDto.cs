using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers
{
    public class LanguageDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = "";
        public string Level { get; set; } = "";
    }
}
