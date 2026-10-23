using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Stats
{
    public interface IStatsService
    {
        Task<PlatformStatsDto> GetPlatformStatsAsync(CancellationToken ct = default);

    }
}
