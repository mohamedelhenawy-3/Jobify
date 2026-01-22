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
    public class EmployerRepository : IEmployeer
    {
        private readonly AppDBContext _context;

        public EmployerRepository(AppDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(EmployerProfile employer)
        {
            await _context.EmployerProfiles.AddAsync(employer);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
