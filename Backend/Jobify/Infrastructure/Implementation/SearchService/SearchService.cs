using Application.Interfaces.Company;
using Application.Interfaces.Job;
using Application.Interfaces.JobSeeker;
using Application.SearchFeature.Dtos;
using Application.SearchFeature.Interface;
using Infrastructure.DBContext;
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

        public SearchService(AppDBContext db, IJobService js, IJobSeekerService sj,ICompanyService cs)
        {
            _db = db;
            _companysrvice = cs;
            _jobService = js;
            _seekerservice = sj;
        }
        public async Task<GlobalSearchResponse> GlobalSearchAsync(GlobalSearchDto dto)
        {
           var response = new GlobalSearchResponse();
           bool onlyKeyword =!string.IsNullOrEmpty(dto.Keyword) && dto.IndustryIds == null && dto.WorkModeIds == null &&
           dto.EmploymentTypeIds == null &&
           dto.CompanySize == null &&
           dto.Industry == null &&
           dto.PostedWithinDays == null;

            if (onlyKeyword || dto.Type == "all")
            {
                var jobsTask = await _jobService.SearchJobsAsync(dto);
                var usersTask = await _seekerservice.SearchUsersAsync(dto);
                var companiesTask = await _companysrvice.SearchCompaniesAsync(dto);

           

                response.Jobs = jobsTask;
                response.Users = usersTask;
                response.Companies = companiesTask;

                return response;
            }
            switch (dto.Type)
            {
                case "job":
                    response.Jobs = await _jobService.SearchJobsAsync(dto);
                    break;

                case "user":
                    response.Users = await _seekerservice.SearchUsersAsync(dto);
                    break;

                case "company":
                    response.Companies = await _companysrvice.SearchCompaniesAsync(dto);
                    break;
            }

            return response;
        }
    }
}
