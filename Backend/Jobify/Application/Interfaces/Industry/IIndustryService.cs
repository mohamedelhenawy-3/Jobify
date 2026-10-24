using Application.DTOs.Industries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Industry
{
    public interface IIndustryService
    {
        Task<List<IndustryDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default);
        Task<IndustryDto> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IndustryDto> CreateAsync(CreateIndustryDto dto, CancellationToken ct = default);
        Task UpdateAsync(int id, UpdateIndustryDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);

        Task<List<PopularIndustryDto>> GetPopularAsync(int take = 6);



    }
}
