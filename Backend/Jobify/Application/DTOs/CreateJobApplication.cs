using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateJobApplication
    {

        public string? CoverLetter { get; set; }
        public string? ExperienceSummary { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Motivation { get; set; }
        public IFormFile? Resume { get; set; }

        

    }
}
