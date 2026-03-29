using Application.DTOs;
using Application.DTOs.JobApplication;
using Application.DTOs.SuccefullOp;
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
        Task<SuccessResponse> UpdateJobStatus(UpdateAppliedJobStatus updatestatusdto, int applicationId);
        //Task<SuccessResponse> UpdateJobInterviewStatus(InterviewStatus updatestatusdto, int applicationId);

    }
}
