using Application.Interfaces.Email;
using Domain.Entities;
using Domain.Entities.Event;
using Domain.Entities.LooksUp;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
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
        public async Task Handler(ApplicationStatusEvent data)
        {
            var app = data.Application;
            var interview = data.Interview;

            var jobSeeker = await _db.JobSeekers
                .Include(js => js.User)
                .FirstOrDefaultAsync(js => js.Id == app.JobSeekerId);

            if (jobSeeker == null || jobSeeker.User == null)
                return;

            

            if (app.Status == ApplicationStatus.Interviewed && interview != null)
            
                {
                var body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
  <meta charset='UTF-8'>
  <meta name='viewport' content='width=device-width, initial-scale=1.0'>
  <title>Interview Invitation</title>
</head>
<body style='font-family: Arial, sans-serif; background-color: #f5f6fa; margin:0; padding:0;'>
  <table width='100%' cellpadding='0' cellspacing='0' style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 10px; padding: 20px; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>
    <tr>
      <td style='text-align: center; padding-bottom: 20px;'>
        <h1 style='color: #4f46e5; margin:0; font-size: 24px;'>🎉 Interview Invitation</h1>
      </td>
    </tr>
    <tr>
      <td style='padding: 10px 0; font-size: 16px; color: #333333; line-height: 1.5;'>
        <p>Dear Candidate,</p>
        <p>Congratulations! You have been selected for an interview. Below are the details:</p>
      </td>
    </tr>
    <tr>
      <td style='padding: 10px 0;'>
        <table width='100%' cellpadding='0' cellspacing='0' style='border-collapse: collapse;'>
          <tr>
            <td style='padding: 10px; background-color: #eef2ff; border-radius: 6px;'>
              <p style='margin: 5px 0;'><strong>Date:</strong> {interview.InterviewDate:dddd, MMM dd yyyy}</p>
              <p style='margin: 5px 0;'><strong>Time:</strong> {interview.InterviewDate:hh:mm tt}</p>
              <p style='margin: 5px 0;'><strong>Location:</strong> {interview.Location}</p>
              {(string.IsNullOrEmpty(interview.Notes) ? "" : $"<p style='margin:5px 0;'><strong>Notes:</strong> {interview.Notes}</p>")}
            </td>
          </tr>
        </table>
      </td>
    </tr>
    <tr>
      <td style='padding: 20px 0; text-align: center;'>
        <a href='#' style='background-color: #4f46e5; color: white; text-decoration: none; padding: 12px 24px; border-radius: 6px; font-weight: bold;'>View Details</a>
      </td>
    </tr>
    <tr>
      <td style='padding: 10px 0; font-size: 14px; color: #6b7280; text-align: center;'>
        <p>If you have any questions, please contact us at <a href='mailto:hr@company.com'>hr@company.com</a></p>
      </td>
    </tr>
    <tr>
      <td style='padding: 10px 0; font-size: 12px; color: #9ca3af; text-align: center;'>
        <p>&copy; {DateTime.Now.Year} Your Company Name. All rights reserved.</p>
      </td>
    </tr>
  </table>
</body>
</html>
";


                await _emailService.SendEmail(
                    jobSeeker.User.Email,
                    "Interview Invitation",
                    body
                    );
            }
        }
    }
}
