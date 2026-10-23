using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO.RecordsDTO
{
    public record LatestJobDto(
      int Id,
      string Title,
      string CompanyName,
      string? CompanyLogoUrl,
      string Location,
      string JobType,
      decimal? SalaryMin,
      decimal? SalaryMax,
      DateTime CreatedAt
  );
}
