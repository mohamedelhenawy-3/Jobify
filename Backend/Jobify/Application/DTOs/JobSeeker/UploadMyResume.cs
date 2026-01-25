using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker
{
    public class UploadMyResume
    {
        public IFormFile? Resume { get; set; }

    }
}
