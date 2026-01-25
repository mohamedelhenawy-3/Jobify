using Application.DTOs.JobSeeker;
using Application.DTOs.SuccefullOp;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.JobSeeker
{
    public interface IJobSeekerService
    {
        Task <SuccessResponse> UploadCv(UploadMyResume resumedto);
        Task<ResumeDto> ViewMyCv();
        Task<JobSeekerProfile> GetJobSeekerProfile(int id);
        Task<MyProfileDto> GetMyProfile();
        Task<SuccessResponse> UpdateMyProfile(UpdateMyProfile updateprofiledto);
    }
}
