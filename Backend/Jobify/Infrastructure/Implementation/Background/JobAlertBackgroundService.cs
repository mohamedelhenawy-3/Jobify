using Application.Interfaces.Email;
using Infrastructure.DBContext;
using Infrastructure.Implementation.Email;
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
                _Log.LogInformation("BackgroundService Created");

                await CheckAlerts();
                _Log.LogInformation("Checks OK");
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task CheckAlerts()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var emailservice = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var  template = scope.ServiceProvider.GetRequiredService<JobEmailTemplate>();

            var alerts = db.SavedJobAlerts.ToList();


            foreach (var alert in alerts)
            {
                var newJobs = db.JobPosts
          .Where(j => j.Title.Contains(alert.Keyword)
                   && j.CreatedAt > alert.LastChecked)
          .ToList();

                if (newJobs.Any())
                {
                    var user =  db.Users.FirstOrDefault(u => u.Id == alert.UserId);
                    var body = template.BuildEmailBody(newJobs, user.FullName);
                    _Log.LogInformation("Found Jobs Ok And Sending Email");
                    await emailservice.SendEmail(user.Email, "New Jobs For User", body);

                }
                alert.LastChecked = DateTime.UtcNow;

            }
            await db.SaveChangesAsync();

        }

    }
}
