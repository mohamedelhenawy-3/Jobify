using Application.DTOs;
using Application.Interfaces.Stats;
using Domain.Entities.LooksUp;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.StatsService
{
    public class StatsService :IStatsService
    {
        private const string CacheKey = "platform_stats";
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        private readonly AppDBContext _db;
        private readonly IMemoryCache _cache;

        public StatsService(AppDBContext db, IMemoryCache cache)
        {
            _db = db;
            _cache = cache;
        }

        public async Task<PlatformStatsDto> GetPlatformStatsAsync(CancellationToken ct = default)
        {
            if (_cache.TryGetValue(CacheKey, out PlatformStatsDto? cached) && cached is not null)
                return cached;

            var activeJobs = await _db.JobPosts.CountAsync(j => j.IsActive , ct);
            var companies = await _db.Companies.CountAsync(ct);
            var RegisteredUsers = await _db.Users.CountAsync(ct);
            var hires = await _db.JobApplications.CountAsync(a => a.Status == ApplicationStatus.Hired, ct);

            var stats = new PlatformStatsDto(activeJobs, companies, RegisteredUsers, hires);

            _cache.Set(CacheKey, stats, CacheDuration);
            return stats;
        }
    }
}
