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
        Task SaveJobAsync( int jobId);
        Task UnsaveJobAsync( int jobId);
        Task<bool> IsJobSavedAsync(int jobId);
        Task<List<JobPost>> GetSavedJobsAsync();
    }
}
