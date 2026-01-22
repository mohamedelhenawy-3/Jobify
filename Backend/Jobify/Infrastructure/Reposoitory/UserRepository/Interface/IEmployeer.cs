using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reposoitory.UserRepository.Interface
{
    public interface IEmployeer
    {
        Task AddAsync(EmployerProfile employer);
        Task SaveChangesAsync();
    }
}
