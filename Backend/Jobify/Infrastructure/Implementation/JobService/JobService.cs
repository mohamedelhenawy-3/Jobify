using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.JobPost;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Job;
using Domain.Entities;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.JobService
{
    public class JobService :IJobService
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        public JobService(AppDBContext db,IHttpContextAccessor http)
        {
            _db = db;
            _httpaccessor = http;
        }

        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        public async   Task<SuccessResponse> CreateJob(CreateJob createjobdto)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");
            var employerProfile = await _db.EmployerProfiles
                .FirstOrDefaultAsync(e => e.UserId == user);
            var Job = new JobPost
            {

                Title = createjobdto.Title,
                Description = createjobdto.Description,
                Location = createjobdto.Location,
                MinSalary = createjobdto.MinSalary,
                MaxSalary = createjobdto.MaxSalary,
                IndustryId = createjobdto.IndustryId,
                EmploymentTypeId = createjobdto.EmploymentTypeId,
                StatusId = createjobdto.StatusId,
                EmployerProfileId = employerProfile.Id,
                CreatedByUserId = user,
                CreatedAt = DateTime.UtcNow
            };
            await _db.JobPosts.AddAsync(Job);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Job Added successfully."
            };

        }

        public async Task<JobDto> GetJobById(int id)
        {
            var job =  _db.JobPosts.FirstOrDefault(x => x.Id == id);
            if (job == null) return null;
            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                MinSalary = job.MinSalary,
                MaxSalary = job.MaxSalary,
                StatusId = job.StatusId,
                IndustryId = job.IndustryId,
                EmploymentTypeId = job.EmploymentTypeId
            };
            
        }

        public async Task<IEnumerable<JobDto>> AllJobs()
        {
            return await _db.JobPosts
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Location = j.Location,
                    MinSalary = j.MinSalary,
                    MaxSalary = j.MaxSalary,
                    IndustryId = j.IndustryId,
                    EmploymentTypeId = j.EmploymentTypeId,
                    StatusId = j.StatusId
                })
                .ToListAsync();
        }

        public async Task<SuccessResponse> UpdateJob(UpdateJobDto updatejob, int id)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");

            var employer = await _db.EmployerProfiles
                            .FirstOrDefaultAsync(e => e.UserId == user);
            var job = await _db.JobPosts
                   .FirstOrDefaultAsync(j =>
                       j.Id == id &&
                       j.EmployerProfileId == employer.Id);

            job.Title = updatejob.Title;
            job.Location = updatejob.Location;
            job.MaxSalary = updatejob.MaxSalary;
            job.MinSalary = updatejob.MinSalary;
            job.IndustryId = updatejob.IndustryId;
            job.StatusId = updatejob.StatusId;
            job.EmploymentTypeId = updatejob.EmploymentTypeId;


           await _db.SaveChangesAsync();
            return new SuccessResponse
            {
                Message = "Update Succefully"
            };

        }

        public async Task<SuccessResponse> DeleteJob(int id)
        {
            var user = GetCurrentUserId();
            if (user == null) throw new Exception("User Null");

            var employer = await _db.EmployerProfiles
                            .FirstOrDefaultAsync(e => e.UserId == user);
            var job = await _db.JobPosts
                   .FirstOrDefaultAsync(j =>
                       j.Id == id &&
                       j.EmployerProfileId == employer.Id);

            _db.JobPosts.Remove(job);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Deleted Succefully"
            };

        }

        public async Task<IEnumerable<JobDto>> AllJobsOfEmployer(int employerId)
        {
            var jobs = await _db.JobPosts
                  .Where(j => j.EmployerProfileId == employerId)
                  .Select(j => new JobDto
                  {
                      Id = j.Id,
                      Title = j.Title,
                      Description = j.Description,
                      Location = j.Location,
                      MinSalary = j.MinSalary,
                      MaxSalary = j.MaxSalary,
                      IndustryId = j.IndustryId,
                      EmploymentTypeId = j.EmploymentTypeId,
                      StatusId = j.StatusId
                  })
                  .ToListAsync();

            return jobs;
        }
    }
}
