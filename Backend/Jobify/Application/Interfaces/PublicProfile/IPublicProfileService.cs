using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.Employer;
using Application.DTOs.JobSeeker.JobSeekers;
using Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.PublicProfile
{
    public interface IPublicProfileService
    {
        Task<PublicUserProfileDto?> GetUserProfileAsync(string userId);
        Task<PublicEmployerProfileDto?> GetEmployerByIdAsync(string employerId);
        Task<PublicJobSeekerProfileDto?> GetJobSeekerByIdAsync(string seekerId);

        Task<PublicCompanyProfileDto?> GetCompanyProfileAsync(int companyId, int maxJobs = 10);

    }
}
