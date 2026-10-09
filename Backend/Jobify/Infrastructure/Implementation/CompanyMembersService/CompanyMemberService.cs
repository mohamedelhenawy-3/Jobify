using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.CompanyMembers;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.CompanyMembers;
using Application.Interfaces.Permisions;
using Application.Interfaces.User;
using Domain.Entities;
using Domain.Entities.LooksUp.EmployerPermisions;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.CLoudinary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.CompanyMembersService
{
    public class CompanyMemberService : ICompanyMemberService
    {
        private readonly AppDBContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly IEmployerPermissionService _permissionService;
        private CloudinaryService _cloudinaryService;


        public CompanyMemberService(
            AppDBContext context,
            ICurrentUser currentUser,
            IEmployerPermissionService permissionService,
              CloudinaryService cloudinaryService)
        {
            _context = context;
            _currentUser = currentUser;
            _permissionService = permissionService;
            _cloudinaryService = cloudinaryService;
        }
        private async Task<EmployerProfile> GetActorAsync() => await _permissionService.GetCurrentEmployerWithPermissionAsync(EmployerPermissions.ManageEmployerRoles) ?? throw new UnauthorizedAccessException("You don't have access to manage the team.");
        private async Task<EmployerProfile> GetTargetAsync(EmployerProfile actor, int id)
        {
            var target = await _context.EmployerProfiles
                      .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == actor.CompanyId)   // same company only
                      ?? throw new KeyNotFoundException("Member not found.");

            if (target.Id == actor.Id) throw new InvalidOperationException(" Cant do anything to your self");
            if (target.Role == EmployerCompanyRole.Owner)
                throw new InvalidOperationException("The owner can't be modified. Transfer ownership first.");
            if (actor.Role == EmployerCompanyRole.Admin && target.Role == EmployerCompanyRole.Admin) throw new InvalidOperationException("Admin cant modify other admins only owners can.");
            if (Rank(actor.Role) <= Rank(target.Role))
                throw new UnauthorizedAccessException("You can only manage members ranked below you.");
            return target;
        }
        private static bool CanGrant(EmployerProfile actor, string permission) => actor.Role == EmployerCompanyRole.Owner || actor.Permissions.Contains(permission);

        private static int Rank(EmployerCompanyRole? role) => role switch
        {
            EmployerCompanyRole.Owner => 3,
            EmployerCompanyRole.Admin => 2,
            _ => 1                         // Recruiter and Employer
        };

        private static void Unlink(EmployerProfile e)
        {
            e.CompanyId = null;
            e.Role = EmployerCompanyRole.Employer;
            e.Permissions = new List<string>();
            e.JoinedCompanyAt = null;
        }


        public async Task<SuccessResponseDto> ChangeRoleAsync(int memberId, ChangeMemberRoleDto dto)
        {

            var actor = await GetActorAsync();
            var target = await GetTargetAsync(actor, memberId);


            if (dto.NewRole == EmployerCompanyRole.Owner)
                throw new InvalidOperationException("Use transfer ownership to assign a new owner.");
            if (Rank(dto.NewRole) >= Rank(actor.Role))
                throw new UnauthorizedAccessException("You can only assign roles below your own.");
            if (target.Role == dto.NewRole)
                throw new InvalidOperationException("The member already has this role.");



            target.Role = dto.NewRole;
            target.Permissions = dto.NewRole.DefaultPermissions()
                                 .Where(p => CanGrant(actor, p))      // never give more than the actor has
                                   .ToList();

            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = $"Role changed to {dto.NewRole}." };
        }


        public async Task<SuccessResponseDto> RemoveMemberAsync(int targetEmployerId)
        {
            var acting = await _context.EmployerProfiles
               .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId);
            if (acting?.CompanyId == null)
                return new SuccessResponseDto { Message = "Employer profile not found." };


            var target = await _context.EmployerProfiles.FindAsync(targetEmployerId);
            if (target == null || target.CompanyId != acting.CompanyId)
                return new SuccessResponseDto { Message = "Employer not found in your company." };


            if (!_permissionService.HasPermission(acting, EmployerPermissions.ManageEmployerRoles))
                return new SuccessResponseDto { Message = "You do not have permission to remove members." };


            if (target.Id == acting.Id)
                return new SuccessResponseDto { Message = "Use leave company instead." };

            if (target.Role == EmployerCompanyRole.Owner)
                return new SuccessResponseDto { Message = "The Owner cannot be removed. Transfer ownership first." };

            if (target.Role == EmployerCompanyRole.Admin && acting.Role != EmployerCompanyRole.Owner)
                return new SuccessResponseDto
                {
                    Message = "Only the Owner can remove Admins."
                };

            Unlink(target);
            await _context.SaveChangesAsync();
            return new SuccessResponseDto { Message = "Member removed from the company." };




        }

        public async Task<SuccessResponseDto> LeaveCompanyAsync()
        {
            var me = await _context.EmployerProfiles
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId);
            if (me?.CompanyId == null)
                return new SuccessResponseDto { Message = "You are not in a company." };

            if (me.Role == EmployerCompanyRole.Owner)
                return new SuccessResponseDto { Message = "The Owner cannot leave. Transfer ownership first." };

            Unlink(me);
            await _context.SaveChangesAsync();
            return new SuccessResponseDto { Message = "You left the company." };
        }
        public async Task<SuccessResponseDto> TransferOwnershipAsync(int newOwnerEmployerId)
        {
            var acting = await _context.EmployerProfiles
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId);
            if (acting?.CompanyId == null || acting.Role != EmployerCompanyRole.Owner)
                return new SuccessResponseDto { Message = "Only the Owner can transfer ownership." };

            var target = await _context.EmployerProfiles.FindAsync(newOwnerEmployerId);
            if (target == null || target.CompanyId != acting.CompanyId || !target.IsActive)
                return new SuccessResponseDto { Message = "Employer not found in your company." };

            if (target.Id == acting.Id)
                return new SuccessResponseDto { Message = "You are already the Owner." };

            // one SaveChanges = one transaction, so there is never a moment with zero or two Owners
            target.Role = EmployerCompanyRole.Owner;
            target.Permissions = EmployerCompanyRole.Owner.DefaultPermissions();
            acting.Role = EmployerCompanyRole.Admin;
            acting.Permissions = EmployerCompanyRole.Admin.DefaultPermissions();
            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = "Ownership transferred. You are now an Admin." };
        }




        public async Task<IEnumerable<TeamMemberDto>?> GetCompanyTeamAsync()
        {
            var employer = await _permissionService.GetCurrentEmployerAsync();
            if (employer == null) return null;

            var members = await _context.EmployerProfiles
                .Include(e => e.User)
                .Where(e => e.CompanyId == employer.CompanyId)
                .ToListAsync();

            return members.Select(m => new TeamMemberDto
            {
                Id = m.Id,
                FullName = m.User?.FullName ?? "Unknown",
                Email = m.User?.Email ?? "",
                //Role = m.Role.ToString(),
                //Initials = GetInitials(m.User?.FullName)
            });
        }
        private static string GetInitials(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "?";
            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length switch
            {
                0 => "?",
                1 => parts[0][..Math.Min(2, parts[0].Length)].ToUpper(),
                _ => $"{parts[0][0]}{parts[^1][0]}".ToUpper()
            };
        }




        public async Task<CompanyProfileDto> GetMyCompanyAsync()
        {
            var me = await _context.EmployerProfiles.Include(X => X.User)
                    .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId);

            if (me?.CompanyId == null)
                return null;

            var company = await _context.Companies
                .Include(x => x.Logo)
                .FirstOrDefaultAsync(c => c.Id == me.CompanyId);

            if (company == null)
                return null;

            return new CompanyProfileDto
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                Description = company.Description,
                Founded = company.Founded,
                Industry = company.Industry,
                CompanySize = company.CompanySize,
                Location = company.Location,
                CompanyWebsite = company.CompanyWebsite,
                Logo = company.Logo?.ImageUrl,
                IsVerified = company.IsVerified,
                MyRole = me.Role.ToString(),
                Permissions = me.Permissions.ToList(),
                CanEdit = me.Permissions.Contains(EmployerPermissions.ManageCompanyProfile)
            };

        }

        public async Task<CompanyProfileDto> UpdateMyCompanyAsync(UpdateCompanyDto dto)
        {
            var employerProfile = await _context.EmployerProfiles
                .FirstOrDefaultAsync(x => x.UserId == _currentUser.UserId);

            if (employerProfile == null || employerProfile.CompanyId == null)
                throw new KeyNotFoundException("You do not belong to a company.");

            // Only the owner (ManageCompanyProfile) can update
            if (!employerProfile.Permissions.Contains(EmployerPermissions.ManageCompanyProfile))
                throw new UnauthorizedAccessException("Only the company owner can update the profile.");

            var company = await _context.Companies
                .Include(c => c.Logo)
                .FirstOrDefaultAsync(c => c.Id == employerProfile.CompanyId);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            // Basic fields
            if (!string.IsNullOrWhiteSpace(dto.CompanyName))
                company.CompanyName = dto.CompanyName;

            company.CompanyWebsite = dto.CompanyWebsite;   // direct assignment so they can be cleared
            company.Description = dto.Description;
            company.Location = dto.Location;
            company.Industry = dto.Industry;
            company.CompanySize = dto.CompanySize;
            company.Founded = dto.Founded;

            // Logo
            if (dto.Logo != null)
            {
                if (!dto.Logo.ContentType.StartsWith("image/") || dto.Logo.Length > 2 * 1024 * 1024)
                    throw new ArgumentException("Logo must be an image under 2 MB.");

                // 1. upload the new one first
                var (url, publicId) = await _cloudinaryService.UploadImageAsync(dto.Logo);

                var oldPublicId = company.Logo?.PublicId;

                // 2. save it on the company
                if (company.Logo == null)
                {
                    company.Logo = new ProfileImage
                    {
                        ImageUrl = url,
                        PublicId = publicId,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    company.Logo.ImageUrl = url;
                    company.Logo.PublicId = publicId;
                    company.Logo.CreatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // 3. delete the old image only after everything succeeded
                if (!string.IsNullOrEmpty(oldPublicId))
                    await _cloudinaryService.DeleteImageAsync(oldPublicId);
            }
            else
            {
                await _context.SaveChangesAsync();
            }

            return await GetMyCompanyAsync();
        }

        public async Task<CompanyMembersPreviewDto?> GetMembersAsync(int take = 5)
        {
            var employer = await _permissionService.GetCurrentEmployerAsync();
            if (employer?.CompanyId == null)
                return null;

            take = Math.Clamp(take, 1, 20);

            bool canSeeEmails = employer.Permissions.Contains(EmployerPermissions.ManageEmployerRoles);
            var query = _context.EmployerProfiles
                .AsNoTracking()
                .Where(e => e.CompanyId == employer.CompanyId);

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.Role == EmployerCompanyRole.Owner ? 0
                            : e.Role == EmployerCompanyRole.Admin ? 1 : 2)
                .ThenBy(e => e.JoinedCompanyAt)
                .Take(take)
                .Select(e => new CompanyMemberDto
                {
                    logo = e.ProfileImage.ImageUrl,
                    EmployerId = e.Id,
                    FullName = e.User.FullName,
                    Email = canSeeEmails ? e.User.Email : null,
                    Role = e.Role.ToString(),
                    Permissions = e.Permissions,
                    IsActive = e.IsActive,
                    JoinedCompanyAt = e.JoinedCompanyAt
                })
                .ToListAsync();

            return new CompanyMembersPreviewDto
            {
                Total = total,
                Items = items
            };
        }



        public async Task<PagedResult<TeamMemberDto>> GetMembers(TeamFilterDto f)
        {
            var actor = await GetActorAsync();
            var q = _context.EmployerProfiles
                      .AsNoTracking()
                      .Where(e => e.CompanyId == actor.CompanyId);
            if (f.Role.HasValue) q = q.Where(e => e.Role == f.Role);
            if (f.IsActive.HasValue) q = q.Where(e => e.IsActive == f.IsActive);


            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var s = f.Search.Trim().ToLower();
                q = q.Where(e => e.User.FullName.ToLower().Contains(s) || e.User.Email.ToLower().Contains(s));

            }
            var total = await q.CountAsync();
            q = (f.SortBy, f.Desc) switch
            {
                ("joined", false) => q.OrderBy(e => e.JoinedCompanyAt).ThenBy(e => e.Id),
                ("joined", true) => q.OrderByDescending(e => e.JoinedCompanyAt).ThenBy(e => e.Id),
                ("jobs", false) => q.OrderBy(e => _context.JobPosts.Count(j => j.PostedByEmployerId == e.Id)).ThenBy(e => e.Id),
                ("jobs", true) => q.OrderByDescending(e => _context.JobPosts.Count(j => j.PostedByEmployerId == e.Id)).ThenBy(e => e.Id),
                ("name", true) => q.OrderByDescending(e => e.User.FullName).ThenBy(e => e.Id),
                ("name", false) => q.OrderBy(e => e.User.FullName).ThenBy(e => e.Id),
                ("role", _) => q.OrderBy(e => e.Role == EmployerCompanyRole.Owner ? 0
                           : e.Role == EmployerCompanyRole.Admin ? 1 : 2)
                .ThenBy(e => e.User.FullName).ThenBy(e => e.Id),
                _ => q.OrderBy(e => e.Role == EmployerCompanyRole.Owner ? 0
                                  : e.Role == EmployerCompanyRole.Admin ? 1 : 2)
                      .ThenBy(e => e.User.FullName).ThenBy(e => e.Id),
            };


            var items = await q
                .Skip((f.PageNumber - 1) * f.PageSize)
                .Take(f.PageSize)
                .Select(e => new TeamMemberDto
                {
                    Id = e.Id,
                    FullName = e.User.FullName,
                    Email = e.User.Email,
                    Role = e.Role.Value,
                    IsActive = e.IsActive,
                    Permissions = e.Permissions,
                    JoinedAt = e.JoinedCompanyAt,
                    IsMe = e.Id == actor.Id,
                    JobsPosted = _context.JobPosts.Count(j => j.CompanyId == e.CompanyId && j.PostedByEmployerId == e.Id)
                })
                .ToListAsync();


            return new PagedResult<TeamMemberDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = f.PageNumber,
                PageSize = f.PageSize
            };

        }


        public async Task SetActive(int memberId, SetActiveDto dto)
        {
            var actor = await GetActorAsync();
            var target = await GetTargetAsync(actor, memberId);

            target.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();


        }


        public async Task<MyTeamRoleDto> GetMyRoleAsync()
        {
            var me = await _context.EmployerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId && e.CompanyId != null && e.IsActive)
                ?? throw new UnauthorizedAccessException("You are not in a company.");

            return new MyTeamRoleDto
            {
                Role = me.Role ?? EmployerCompanyRole.Employer,
                Permissions = me.Permissions.ToList()
            };
        }
        public async Task<SuccessResponseDto> UpdatePermissionsAsync(int memberId, UpdatePermissionsDto dto)
        {
            var actor = await GetActorAsync();
            var target = await GetTargetAsync(actor, memberId);

            var requested = dto.Permissions.Distinct().ToList();

            if (requested.Any(p => !EmployerPermissions.All.Contains(p)))
                throw new InvalidOperationException("Unknown permission.");

            var added = requested.Except(target.Permissions).ToList();
            if (added.Any(p => !CanGrant(actor, p)))
                throw new UnauthorizedAccessException("You can't grant permissions you don't have.");

            target.Permissions = requested;
            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = "Permissions updated." };
        }
    }
}
