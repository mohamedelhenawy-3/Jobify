using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reposoitory.UserRepository.Interface
{
    public interface IJobSeeker
    {
        Task AddAsync(JobSeeker jobSeeker);
        Task SaveChangesAsync();
    }
}
