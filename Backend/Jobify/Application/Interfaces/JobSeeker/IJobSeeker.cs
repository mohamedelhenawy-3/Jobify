using Application.DTOs.JobSeeker;
using Application.DTOs.SuccefullOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.JobSeeker
{
    public interface IJobSeeker
    {

        Task<JobSeekerProfile> GetJobSeekerProfile(int id);
        Task<MyProfileDto> GetMyProfile(int id);
        Task<SuccessResponse> UpdateMyProfile(UpdateMyProfile updateprofiledto, int id);
    }
}
