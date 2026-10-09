using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.SavedJobDto;
using Application.Interfaces.SavedJob;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
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
        private readonly ICurrentUser _currentUser;

        public SavedJobService(AppDBContext context, ICurrentUser currentUser)
        {
            _db = context;
            _currentUser = currentUser;
        }

        public async Task<PagedResult<SavedJobDto>> GetSavedJobsAsync(int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);

            var q = _db.SavedJobs
                .AsNoTracking()
                .Where(x => x.UserId == _currentUser.UserId);

            var total = await q.CountAsync();

            var items = await q
                .OrderByDescending(x => x.CreatedAt).ThenBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SavedJobDto
                {
                    id = x.Job.Id,
                    logo = x.Job.Company.Logo.ImageUrl,
                    title = x.Job.Title,
                    location = x.Job.Location,
                    JobType = x.Job.JobType,
                    ExperienceLevel = x.Job.ExperienceLevel,
                    SavedAt = x.CreatedAt
                })
                .ToListAsync();

            return new PagedResult<SavedJobDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> IsJobSavedAsync(int jobId) =>
            await _db.SavedJobs.AnyAsync(x => x.UserId == _currentUser.UserId && x.JobId == jobId);

        public async Task SaveJobAsync(int jobId)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Log in to save jobs.");

            if (!await _db.JobPosts.AnyAsync(j => j.Id == jobId))
                throw new KeyNotFoundException("Job not found.");

            if (await _db.SavedJobs.AnyAsync(x => x.UserId == userId && x.JobId == jobId))
                return;                                   // already saved, nothing to do

            _db.SavedJobs.Add(new SavedJob { UserId = userId, JobId = jobId, IsActive = true });

            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException) { /* double click: the unique index rejected the 2nd insert, it is already saved */ }
        }

        public async Task UnsaveJobAsync(int jobId)
        {
            var saved = await _db.SavedJobs
                .FirstOrDefaultAsync(x => x.UserId == _currentUser.UserId && x.JobId == jobId);
            if (saved == null) return;

            _db.SavedJobs.Remove(saved);
            await _db.SaveChangesAsync();
        }
    }
}
