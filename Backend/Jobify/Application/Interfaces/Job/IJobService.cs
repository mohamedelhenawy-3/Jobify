using Application.DTOs.industry;
using Application.DTOs.JobPost;
using Application.DTOs.Pagenation;
using Application.DTOs.SuccefullOp;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Job
{
    public interface IJobService
    {
        Task <SuccessResponse> CreateJob(CreateJob createjobdto);
        Task<ReturnedJobDto> GetJobById(int id);
        Task<JobDto> GetJob(int id);
        Task<IEnumerable<ReturnedJobDto>> AllJobsOfEmployer();

        Task<IEnumerable<JobDto>> AllJobs();
        Task<SuccessResponse> UpdateJob(UpdateJobDto updatejob, int id);
        Task<SuccessResponse> DeleteJob(int Id);

        Task<IndustryDto> JobWithIndustry(int id);
        Task<IEnumerable<IndustryDto>> Industries();


       Task<PageDto<JobDto>> FilteredJobs(JobFilterDto jobfiltereddto, int pagesize = 10, int pagenumber = 1);


    }
}
