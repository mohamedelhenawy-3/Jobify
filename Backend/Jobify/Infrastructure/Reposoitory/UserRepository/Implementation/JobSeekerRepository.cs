using Domain.Entities;
using Infrastructure.DBContext;
using Infrastructure.Reposoitory.UserRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reposoitory.UserRepository.Implementation
{
    public class JobSeekerRepository : IJobSeeker
    {
        private readonly AppDBContext _context;

        public JobSeekerRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(JobSeeker jobSeeker)
        {
            await _context.JobSeekers.AddAsync(jobSeeker);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
