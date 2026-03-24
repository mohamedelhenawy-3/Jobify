using Application.Interfaces.Email;
using Domain.Entities;
using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers
{
    public class ApplicationStatusHandler
    {
        private readonly IEmailService _emailService;
        private readonly AppDBContext _db;

        public ApplicationStatusHandler(IEmailService emailService, AppDBContext db)
        {
            _emailService = emailService;
            _db = db;
        }
        public async Task Handler(JobApplication app)
        {
            var user = await _db.Users.FindAsync(app.JobSeekerId);

            if (user == null)
                return;

            if (app.Status == ApplicationStatus.Interviewed)
            {
                var body = $@"
                  <h2>Interview Invitation 🎉</h2>
                  <p>You have been selected for an interview.</p>
                  <p><strong>Date:</strong> {app.InterviewDate}</p>
                   ";

                 await _emailService.SendEmail(
                    user.Email,
                    "Interview Invitation",
                    body
                 );
            }
        }
    }
}
