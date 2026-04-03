using Application.DTOs.Comapny;
using Application.DTOs.JobPost;
using Application.DTOs.JobSeeker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SearchFeature.Dtos
{
    public class GlobalSearchResponse
    {
        public List<SearchResponseDto> Jobs { get; set; } = new();
        public List<UserSeachDto> Users { get; set; } = new();
        public List<SearchCompanyDto> Companies { get; set; } = new();
    }
}
