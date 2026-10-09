using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.SavedJobDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.JobsAlert
{
    public interface IAlertJobs
    {
        Task<SuccessResponseDto> CreateAsync(CreateJobAlertDto dto);
        Task<IEnumerable<JobAlertDto>> GetMineAsync();
        Task<SuccessResponseDto> DeleteAsync(int id);
        Task<SuccessResponseDto> SetActiveAsync(int id, SetAlertActiveDto dto);
    }
}
