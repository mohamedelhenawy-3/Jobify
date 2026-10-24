using Application.DTOs.Comapny.RefactorCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.PublicCompany
{
    public interface IPublicCompanyService
    {
        Task<List<PublicCompanyDto>> SearchAsync(string? q, int? industryId, string? location);
        Task<PublicCompanyDto> GetByIdAsync(int id);
    }
}
