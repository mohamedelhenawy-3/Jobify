using Application.DTOs;
using Application.DTOs.JobApplication;
using Application.DTOs.JobApplication.JobApplication;
using Application.DTOs.SuccefullOp;
using Domain.Entities;
using Domain.Entities.LooksUp.EmployerPermisions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.JobApplication
{
    public interface IJobApplication
    {
        Task<SuccessResponse> CreateApplication(CreateJobApplication jobdto ,int id);
        Task<IEnumerable<MyApplications>> GetMyApplications();
        Task<IEnumerable<AppliedJobSeekers>> GetAppliedJobSeeker();
        Task<AppliedJoSeekerDetails> GetAppliedJobSeekerDeyails(int applicationId);
        //Task<SuccessResponse> UpdateJobInterviewStatus(InterviewStatus updatestatusdto, int applicationId);
        Task<IEnumerable<MyApplicationDto>> GetMyApplications(ApplicationFilterDto filterdto);
        Task<IEnumerable<AppliedJobSeekerDto>> appliedapplications(ApplicationFilterDto filterdto);

        ///////////////////////
        ///

         Task<List<RecentApplicantDto>> GetRecentApplicantsAsync(int take = 5);
        Task<HiringPipelineDto> GetPipelineAsync();
        Task<List<JobApplicationListItemDto>> GetApplicationsForSeekerAsync(JobApplicationFilterDto filter);
        Task<List<JobApplicationListItemDto>> GetEmployerApplicationsAsync(JobApplicationFilterDto filter);
        Task<SuccessResponse> UpdateJobStatus(UpdateAppliedJobStatus updatestatusdto, int applicationId);
        Task<ApplicationDetailsDto> GetApplicationDetails(int applicationId);






    }
}
