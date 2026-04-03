using Application.SearchFeature.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.SearchFeature.Interface
{
    public interface ISearchService
    {
        Task<GlobalSearchResponse> GlobalSearchAsync(GlobalSearchDto dto);

    }
}
