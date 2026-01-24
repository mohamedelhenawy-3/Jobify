using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Employer;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Company
{
    public interface IEmployerService
    {
        Task<EmployerProfile?> GetByIdAsync(int id);
        Task<EmployerProfile?> GetByEmployerIdAsync(string userId);
        Task<List<EmployerProfile>> GetAllAsync();
        Task<SuccessResponseDto> UpdateAsync(UpdateEmployerProfile profile);
    }
}
