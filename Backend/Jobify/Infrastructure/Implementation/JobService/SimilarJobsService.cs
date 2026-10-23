using Application.DTOs.JobPost.JobDTO;
using Application.Interfaces.Job;
using Domain.Entities;
using Domain.Entities.LooksUp.JobEnum;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.JobService
{
    public class SimilarJobsService : ISimilarJobsService
    {
        private readonly AppDBContext _db; // use your DbContext name
        public SimilarJobsService(AppDBContext db) => _db = db;

        public async Task<List<SimilarJobDto>?> GetSimilarAsync(int jobId, int take)
        {
            take = Math.Clamp(take, 1, 20);
            var now = DateTime.UtcNow;

            // Project only what scoring needs
            IQueryable<JobCandidate> Project(IQueryable<JobPost> q) => q.Select(j => new JobCandidate
            {
                Id = j.Id,
                Title = j.Title,
                Location = j.Location,
                JobType = j.JobType,
                Experience = j.ExperienceLevel,
                SalaryMin = j.SalaryMin,
                SalaryMax = j.SalaryMax,
                CreatedAt = j.CreatedAt,
                CompanyId = j.CompanyId,
                CompanyName = j.Company.CompanyName, 
                Logo = j.Company.Logo.ImageUrl,               
                Requirements = j.Requirements,
                Description = j.Description
            });

            var source = await Project(_db.JobPosts.AsNoTracking().Where(j => j.Id == jobId))
                .FirstOrDefaultAsync();
            if (source == null) return null;

            Prepare(source);
            var candidates = await Project(
                    _db.JobPosts.AsNoTracking()
                        .Where(j => j.IsActive && j.Id != jobId && j.ExpirationDate > now)
                        .Where(j => j.JobType == source.JobType
                                 || j.ExperienceLevel == source.Experience
                                 || j.CompanyId == source.CompanyId
                                 || j.Location == source.Location)
                        .OrderByDescending(j => j.CreatedAt)
                        .Take(300))
                .ToListAsync();

            return candidates
                .Select(c =>
                {
                    Prepare(c);
                    return new SimilarJobDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        CompanyName = c.CompanyName,
                        Location = c.Location,
                        JobType = c.JobType.ToString(),
                        ExperienceLevel = c.Experience.ToString(),
                        MinSalary = c.SalaryMin,
                        MaxSalary = c.SalaryMax,
                        Logo = c.Logo,
                        CreatedAt = c.CreatedAt,
                        MatchPercent = SimilarJobScorer.Score(source, c)
                    };
                })
                .Where(x => x.MatchPercent >= 20)      
                .OrderByDescending(x => x.MatchPercent)
                .ThenByDescending(x => x.CreatedAt)
                .Take(take)
                .ToList();
        }

        private static void Prepare(JobCandidate c)
        {
            c.TitleWords = SimilarJobScorer.Keywords(c.Title);
            c.BodyWords = SimilarJobScorer.Keywords(c.Requirements + " " + c.Description);
        }
    }
}
