using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Email
{
    public class JobEmailTemplate
    {
        public string BuildEmailBody(List<JobPost> jobs, string userName)
        {
            var jobsHtml = "";

            foreach (var job in jobs)
            {
                jobsHtml += $@"
            <div style='padding:10px; border-bottom:1px solid #eee;'>
                <h3 style='margin:0;'>{job.Title}</h3>

                <a href='https://yourwebsite.com/jobs/{job.Id}'>
                    View Job
                </a>
            </div>";
            }

            return $@"
        <div style='font-family:Arial'>
            <h2>Hi {userName} 👋</h2>
            <p>We found new jobs for you:</p>

            {jobsHtml}
        </div>";
        }
    }
}
