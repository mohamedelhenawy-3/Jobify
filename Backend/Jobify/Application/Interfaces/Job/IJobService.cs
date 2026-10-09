using Application.DTOs.industry;
using Application.DTOs.JobPost;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.Pagenation;
using Application.DTOs.RecommendedJobbs;
using Application.DTOs.SuccefullOp;
using Application.SearchFeature.Dtos;
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

        //owner -employers
        Task<IEnumerable<ReturnedJobDto>?> GetEmployerJobsAsync();
        Task<PagedResult<EmployerJobCardDto>?> GetEmployerJobsAsync(EmployerJobsFilterDto filter);

        Task<List<EmployerJobItemDto>> GetMyJobsAsync(); //based on employer admin-owner-recruter
        Task<SuccessResponse> UpdateJob(UpdateJobDto dto, int id);
        Task<SuccessResponse> CreateJob(CreateJobPostDto createjobdto);
         Task<JobDetailsForJobseekerDto> GetJobDetailsForJobseeker(int jobId);
        //Task<PagedResult<ReturnedJobDto>> SearchJobs(JobsFilterDto filter);
        Task<PagedResult<JobCardDto>> GetActiveJobsAsync(JobsFilterDto filter);
        //////////////

        Task<ReturnedJobDto> GetJobById(int id);
        Task<JobDto> GetJob(int id);
        Task<IEnumerable<JobDto>> AllJobs();
  
        Task<SuccessResponse> DeleteJob(int Id);
        Task<List<RecommendedJobsDto>> GetRecommendedJobs();
        ///
        Task<SuccessResponse> SaveAlert(string keyword);
        Task<List<JobPost>> FindNewJobs(SavedJobAlert alert);

        Task<List<SearchResponseDto>> Search(JobFilterDto jobfiltereddto);
        ////////////Apply Search Globaly/////////////
        Task<List<SearchResDto>> SearchJobsAsync(GlobalSearchDto dto);


    }
}
