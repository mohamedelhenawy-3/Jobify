using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobPost.JobDTO.RecordsDTO
{
    public record RecordPagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int Size,
    int TotalCount
    );
}
