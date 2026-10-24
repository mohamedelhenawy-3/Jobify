using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Industries
{
    public record IndustryDto(int Id, string Name, string Slug, bool IsActive);
}
