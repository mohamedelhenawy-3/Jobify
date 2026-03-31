using Application.DTOs.industry;
using Application.DTOs.JobPost;
using Application.DTOs.Pagenation;
using Application.DTOs.RecommendedJobbs;
using Application.DTOs.SuccefullOp;
using Domain.Entities;
using Domain.Entities.LooksUp;
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

        Task<List<RecommendedJobsDto>> GetRecommendedJobs();
        ///
        Task<SuccessResponse> SaveAlert(string keyword);
        Task<List<JobPost>> FindNewJobs(SavedJobAlert alert);

        Task<List<OpeningJobs>> AvalibleJobs();



        //NEW PART FOR search globlay
        Task<List<SearchResponseDto>> Search(string keyword);

    }
}
