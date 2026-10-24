using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Industries
{
    public record PopularIndustryDto(int Id, string Name, string Slug, int JobsCount);

}
