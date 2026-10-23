using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO.RecordsDTO
{
    public record RecommendedJobDto(

        int JobId,
        string Title,
        string? LogoUrl,
        string? CompanyName,
        string? Location,
        string EmploymentType,
        decimal? SalaryMin,
        decimal? SalaryMax,
        string? Currency,
        double MatchScore,
        IReadOnlyList<string> MatchedSkills,
        DateTime PostedAtUtc

    );
}
