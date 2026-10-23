using Application.DTOs.CompanyMembers;
using Application.DTOs.JobPost.JobDTO;
using Application.Interfaces.Logger;
using CloudinaryDotNet;
using Domain.Entities;
using Domain.Entities.Comp;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace Infrastructure.Implementation.Logger
{


    public class ActivityLogger : IActivityLogger
    {
        private readonly AppDBContext _context;

        public ActivityLogger(AppDBContext context) => _context = context;

        private async Task<string> NameOfAsync(int employerId) =>
            await _context.EmployerProfiles
                .Where(e => e.Id == employerId)
                .Select(e => e.User.FullName)
                .FirstOrDefaultAsync() ?? "Unknown";

        public async Task LogAsync(int companyId, EmployerProfile? actor, EmployerProfile? target,
       ActivityType type, Func<string, string?, string> message,
       int? refId = null, string? actorNameOverride = null)
        {
            var actorName = actorNameOverride
                ?? (actor == null ? "System" : await NameOfAsync(actor.Id));
            var targetName = target == null ? null : await NameOfAsync(target.Id);

            _context.CompanyActivities.Add(new CompanyActivity
            {
                CompanyId = companyId,
                Type = type,
                ActorEmployerId = actor?.Id,
                ActorName = actorName,
                TargetEmployerId = target?.Id,
                TargetName = targetName,
                Message = message(actorName, targetName),
                RefId = refId,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
        }
    }
}
