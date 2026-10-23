using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Permisions;
using Application.Interfaces.Permisions;
using Domain.Entities;
using Domain.Entities.LooksUp.EmployerPermisions;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.PermisionsService
{
    public class EmployerPermissionService : IEmployerPermissionService
    {

        private readonly AppDBContext _context;
        private readonly ICurrentUser _currentUser;

        public EmployerPermissionService(AppDBContext context, ICurrentUser currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public bool HasPermission(EmployerProfile employer, string permission)
        => employer.IsActive
           && (employer.Role == EmployerCompanyRole.Owner
               || employer.Permissions.Contains(permission));




        public async Task<List<EmployerPermissionsDto>> GetPermissionsAsync(int employerId)
        {
            var employer = await _context.EmployerProfiles.FindAsync(employerId);
            if (employer is null)
                return new List<EmployerPermissionsDto>();

            var permissions = employer.Role == EmployerCompanyRole.Owner
                ? EmployerPermissions.All.ToList()
                : employer.Permissions;

            return new List<EmployerPermissionsDto>
            {
                new EmployerPermissionsDto
                {
                    EmployerId = employer.Id,
                    Role = employer.Role.ToString(),
                    Permissions = permissions
                }
            };
        }

        public async Task<SuccessResponseDto> GrantAsync(int targetEmployerId, string permission)
        {
            if (!EmployerPermissions.All.Contains(permission))
                return new SuccessResponseDto { Message = "Unknown permission." };

            var acting = await _context.EmployerProfiles
                  .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId
                           && e.CompanyId != null
                           && e.IsActive);
            if (acting is null)
                return new SuccessResponseDto { Message = "Employer profile not found." };

            var target = await _context.EmployerProfiles.FindAsync(targetEmployerId);
            if (target is null)
                return new SuccessResponseDto { Message = "Target employer not found." };

            if (acting.CompanyId is null || acting.CompanyId != target.CompanyId)
                return new SuccessResponseDto { Message = "Cannot manage an employer outside your company." };

            if (!HasPermission(acting, EmployerPermissions.ManageEmployerRoles))
                return new SuccessResponseDto { Message = "You do not have permission to manage employer roles." };

            if (!target.Permissions.Contains(permission))
            {
                target.Permissions.Add(permission);
                await _context.SaveChangesAsync();
            }

            return new SuccessResponseDto { Message = "Permission granted." };
        }

        public async Task<SuccessResponseDto> RevokeAsync( int targetEmployerId, string permission)
        {
            var acting = await _context.EmployerProfiles
                           .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId
                           && e.CompanyId != null
                           && e.IsActive);
            if (acting is null)
                return new SuccessResponseDto { Message = "Employer profile not found." };

            var target = await _context.EmployerProfiles.FindAsync(targetEmployerId);
            if (target is null)
                return new SuccessResponseDto { Message = "Target employer not found." };

            if (acting.CompanyId is null || acting.CompanyId != target.CompanyId)
                return new SuccessResponseDto { Message = "Cannot manage an employer outside your company." };

            if (!HasPermission(acting, EmployerPermissions.ManageEmployerRoles))
                return new SuccessResponseDto { Message = "You do not have permission to manage employer roles." };

            if (target.Role == EmployerCompanyRole.Owner)
                return new SuccessResponseDto { Message = "Cannot revoke permissions from an Owner directly; change their Role instead." };

            target.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = "Permission revoked." };
        }




        public async Task<EmployerProfile?> GetCurrentEmployerWithPermissionAsync(string permission)
        {
            var employer = await _context.EmployerProfiles.Include(e => e.User)
                        .Include(x => x.ProfileImage)
                        .Include(e => e.Company)
                            .ThenInclude(c => c!.Logo)
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId
                           && e.CompanyId != null
                           && e.IsActive && e.HasAccess);

            if (employer?.CompanyId == null || !HasPermission(employer, permission))
                return null;

            return employer;
        }

        public  async Task<EmployerProfile?> GetCurrentEmployerAsync()
        {
            var employer = await _context.EmployerProfiles
                        .Include(e => e.User)
                        .Include(x=>x.ProfileImage)
                        .Include(e => e.Company)
                            .ThenInclude(c => c!.Logo)
                          .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId
                           && e.CompanyId != null
                           && e.IsActive);

            return employer?.CompanyId == null ? null : employer;
        }
    }
}

