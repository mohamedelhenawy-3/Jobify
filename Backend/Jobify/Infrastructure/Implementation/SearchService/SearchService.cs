using Application.Interfaces.Company;
using Application.Interfaces.Job;
using Application.Interfaces.JobSeeker;
using Application.SearchFeature.Dtos;
using Application.SearchFeature.Interface;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Reposoitory.UserRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.SearchService
{
    public class SearchService : ISearchService
    {
        private readonly AppDBContext _db;
        private readonly IJobService _jobService;
        private readonly IJobSeekerService _seekerservice;
        private readonly ICompanyService _companysrvice;

        public SearchService(AppDBContext db, IJobService js, IJobSeekerService sj, ICompanyService cs)
        {
            _db = db;
            _companysrvice = cs;
            _jobService = js;
            _seekerservice = sj;
        }
        public async Task<GlobalSearchResponse> GlobalSearchAsync(GlobalSearchDto dto)
        {
            var response = new GlobalSearchResponse();
            var type = (dto.Type ?? "all").Trim().ToLower();

            if (type == "all" || type == "")
            {
                bool jobFilters = !string.IsNullOrWhiteSpace(dto.EmploymentType)
                               || !string.IsNullOrWhiteSpace(dto.ExperienceLevel)
                               || dto.PostedWithinDays.HasValue;

                bool companyFilters = 
                                   !string.IsNullOrWhiteSpace(dto.CompanySize);

                bool includeJobs = !companyFilters || jobFilters;
                bool includeCompanies = !jobFilters || companyFilters;
                bool includeUsers = !jobFilters && !companyFilters;

                if (includeJobs) response.Jobs = await _jobService.SearchJobsAsync(dto);
                if (includeCompanies) response.Companies = await _companysrvice.SearchCompaniesAsync(dto);
                if (includeUsers) response.Users = await _seekerservice.SearchUsersAsync(dto);

                return response;
            }

            switch (type)
            {
                case "job":
                    response.Jobs = await _jobService.SearchJobsAsync(dto);
                    break;
                case "company":
                    response.Companies = await _companysrvice.SearchCompaniesAsync(dto);
                    break;
                case "user":
                    response.Users = await _seekerservice.SearchUsersAsync(dto);
                    break;
            }

            return response;
        }
    }
}
