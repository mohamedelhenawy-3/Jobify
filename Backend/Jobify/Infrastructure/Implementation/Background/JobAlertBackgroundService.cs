using Application.Interfaces.Email;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IEmailService = Application.Interfaces.Email.IEmailService;

namespace Infrastructure.Implementation.Background
{
    public class JobAlertBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<JobAlertBackgroundService> _Log;

        public JobAlertBackgroundService(IServiceScopeFactory scopeFactory, ILogger<JobAlertBackgroundService> Log)
        {
            _scopeFactory = scopeFactory;
            _Log = Log;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try { await CheckAlerts(stoppingToken); }
                catch (Exception ex) { _Log.LogError(ex, "Job alert check failed"); }  

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task CheckAlerts(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var template = scope.ServiceProvider.GetRequiredService<JobEmailTemplate>();

            var now = DateTime.UtcNow;   // captured once, before querying

            var alerts = await db.SavedJobAlerts.Where(a => a.IsActive).ToListAsync(ct);

            foreach (var alert in alerts)
            {
                try
                {
                    var newJobs = await db.JobPosts
                        .AsNoTracking()
                        .Where(j => j.CreatedAt > alert.LastChecked
                                 && j.CreatedAt <= now
                                 && j.Title.Contains(alert.Keyword))
                        .OrderByDescending(j => j.CreatedAt)
                        .Take(20)
                        .ToListAsync(ct);

                    if (newJobs.Count > 0)
                    {
                        var user = await db.Users.AsNoTracking()
                            .FirstOrDefaultAsync(u => u.Id == alert.UserId, ct);

                        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
                        {
                            var body = template.BuildEmailBody(newJobs, user.FullName);
                            await emailService.SendEmail(user.Email, $"New jobs for \"{alert.Keyword}\"", body);
                            _Log.LogInformation("Alert {Id} sent to {Email}", alert.Id, user.Email);
                        }
                    }

                    alert.LastChecked = now;              
                    await db.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    _Log.LogError(ex, "Alert {Id} failed, will retry next cycle", alert.Id);
                }
            }
        }
     }
}
