using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.SavedJobDto;
using Application.Interfaces.JobsAlert;
using Domain.Entities.LooksUp;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.AlertJobs
{
    public class AlertJobsService : IAlertJobs
    {
        private const int MaxAlertsPerUser = 10;
        private readonly AppDBContext _context;
        private readonly ICurrentUser _currentUser;

        public AlertJobsService(AppDBContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        public async Task<SuccessResponseDto> CreateAsync(CreateJobAlertDto dto)
        {
            var keyword = (dto.Keyword ?? "").Trim().ToLower();
            if (keyword.Length < 2 || keyword.Length > 100)
                throw new ArgumentException("Keyword must be between 2 and 100 characters.");

            var mine = _context.SavedJobAlerts.Where(a => a.UserId == _currentUser.UserId);

            if (await mine.AnyAsync(a => a.Keyword == keyword))
                throw new InvalidOperationException("You already have an alert for this keyword.");

            if (await mine.CountAsync() >= MaxAlertsPerUser)
                throw new InvalidOperationException($"You can have up to {MaxAlertsPerUser} alerts.");

            _context.SavedJobAlerts.Add(new SavedJobAlert
            {
                UserId = _currentUser.UserId,
                Keyword = keyword,
                LastChecked = DateTime.UtcNow, 
                IsActive = true
            });
            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = $"Alert saved. We'll email you new \"{keyword}\" jobs." };
        }

        public async Task<IEnumerable<JobAlertDto>> GetMineAsync() =>
            await _context.SavedJobAlerts.AsNoTracking()
                .Where(a => a.UserId == _currentUser.UserId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new JobAlertDto { Id = a.Id, Keyword = a.Keyword, IsActive = a.IsActive, CreatedAt = a.CreatedAt })
                .ToListAsync();

        public async Task<SuccessResponseDto> SetActiveAsync(int id, SetAlertActiveDto dto)
        {
            var alert = await _context.SavedJobAlerts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == _currentUser.UserId)   // own alerts only
                ?? throw new KeyNotFoundException("Alert not found.");

            if (alert.IsActive == dto.IsActive)
                return new SuccessResponseDto { Message = dto.IsActive ? "Alert is already on." : "Alert is already paused." };
            if (dto.IsActive)
                alert.LastChecked = DateTime.UtcNow;

            alert.IsActive = dto.IsActive;
            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = dto.IsActive ? "Alert turned on." : "Alert paused." };
        }
        public async Task<SuccessResponseDto> DeleteAsync(int id)
        {
            var alert = await _context.SavedJobAlerts
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == _currentUser.UserId)   // own alerts only
                ?? throw new KeyNotFoundException("Alert not found.");

            _context.SavedJobAlerts.Remove(alert);
            await _context.SaveChangesAsync();
            return new SuccessResponseDto { Message = "Alert deleted." };
        }
    }
}
