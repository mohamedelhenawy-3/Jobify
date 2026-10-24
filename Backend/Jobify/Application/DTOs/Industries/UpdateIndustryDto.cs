using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Industries
{
    public record UpdateIndustryDto([Required, StringLength(100)] string Name, bool IsActive);
}
