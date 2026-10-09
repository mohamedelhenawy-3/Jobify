using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Permisions;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Permisions
{
    public interface IEmployerPermissionService
    {

        Task<EmployerProfile?> GetCurrentEmployerWithPermissionAsync(string permission);
        Task<EmployerProfile?> GetCurrentEmployerAsync();

        bool HasPermission(EmployerProfile employer, string permission);
        Task<SuccessResponseDto> GrantAsync(int targetEmployerId, string permission);
        Task<SuccessResponseDto> RevokeAsync(int targetEmployerId, string permission);
        Task<List<EmployerPermissionsDto>> GetPermissionsAsync(int employerId);
    }
}

