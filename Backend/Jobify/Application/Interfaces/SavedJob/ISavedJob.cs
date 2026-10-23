using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.SavedJobDto;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.SavedJob
{
    public interface ISavedJob
    {
        Task<PagedResult<SavedJobDto>> GetSavedJobsAsync(int pageNumber, int pageSize);
        Task<bool> IsJobSavedAsync(int jobId);
        Task SaveJobAsync(int jobId);
        Task UnsaveJobAsync(int jobId);


        ////////////////
        ///

    }
}
