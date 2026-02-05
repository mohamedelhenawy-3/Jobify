using Application.Interfaces.SavedJob;
using Domain.Entities;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.SavedJobService
{
    public class SavedJobService : ISavedJob
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;

        public SavedJobService(
            AppDBContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = context;
            _httpaccessor = httpContextAccessor;
        }

        private string? GetCurrentUserId()
        {
            return _httpaccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async  Task<List<JobPost>> GetSavedJobsAsync()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();
            return await _db.SavedJobs
                       .Where(x => x.UserId == userId)
                       .Select(x => x.Job)
                       .ToListAsync();
        }

        public async  Task<bool> IsJobSavedAsync(int jobId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();
            return await _db.SavedJobs
                       .AnyAsync(x => x.UserId == userId && x.JobId == jobId);
        }

        public async Task SaveJobAsync(int jobId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

           
            if (await _db.SavedJobs
                     .AnyAsync(x => x.UserId == userId && x.JobId == jobId))
                return;

            _db.SavedJobs.Add(new Domain.Entities.SavedJob
            {
                UserId = userId,
                JobId = jobId
            });

            await _db.SaveChangesAsync();
        }

        public async Task UnsaveJobAsync(int jobId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();
            var savedJob = await _db.SavedJobs
         .FirstOrDefaultAsync(x => x.UserId == userId && x.JobId == jobId);

            if (savedJob == null) return;

            _db.SavedJobs.Remove(savedJob);
            await _db.SaveChangesAsync();
        }
    }
}
