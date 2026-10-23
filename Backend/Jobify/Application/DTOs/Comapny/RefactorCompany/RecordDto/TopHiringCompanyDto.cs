using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny.RefactorCompany.RecordDto
{
   
    public record TopHiringCompanyDto(
    int Id,
    string Name,
    string? LogoUrl,
    string? Industry,
    string? Location,
    int OpenJobs
    );
   
}
