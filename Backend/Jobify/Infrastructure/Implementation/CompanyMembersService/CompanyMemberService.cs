using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny.RefactorCompany;
using Application.DTOs.CompanyMembers;
using Application.DTOs.JobPost.JobDTO;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.CompanyMembers;
using Application.Interfaces.Logger;
using Application.Interfaces.Permisions;
using Application.Interfaces.User;
using Domain.Entities;
using Domain.Entities.Comp;
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

        private readonly IActivityLogger _activity;


        public CompanyMemberService(
            AppDBContext context,
            ICurrentUser currentUser,
            IActivityLogger activitu,
            IEmployerPermissionService permissionService,
              CloudinaryService cloudinaryService)
        {
            _context = context;
            _currentUser = currentUser;
            _permissionService = permissionService;
            _cloudinaryService = cloudinaryService;
            _activity = activitu;
        }
        private async Task<string> NameOfAsync(int employerId) => await _context.EmployerProfiles
        .Where(e => e.Id == employerId)
        .Select(e => e.User.FullName)
        .FirstOrDefaultAsync() ?? "Unknown";

        private async Task LogAsync(int companyId, EmployerProfile actor, EmployerProfile? target,
                            ActivityType type, Func<string, string?, string> message)
        {
            var actorName = await NameOfAsync(actor.Id);
            var targetName = target == null ? null : await NameOfAsync(target.Id);

            _context.CompanyActivities.Add(new CompanyActivity
            {
                CompanyId = companyId,
                Type = type,
                ActorEmployerId = actor.Id,
                ActorName = actorName,
                TargetEmployerId = target?.Id,
                TargetName = targetName,
                Message = message(actorName, targetName),
                CreatedAt = DateTime.UtcNow
            });
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
                                 .Where(p => CanGrant(actor, p))      
                                   .ToList();
            await _activity.LogAsync(actor.CompanyId!.Value, actor, target, ActivityType.RoleChanged,(a, t) => $"{a} changed {t}'s role from {target.Role} to {dto.NewRole}.");
            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = $"Role changed to {dto.NewRole}." };
        }


        public async Task<SuccessResponseDto> RemoveMemberAsync(int targetEmployerId, RemoveMemberDto dto)
        {
            var actor = await GetActorAsync();                     
            var target = await GetTargetAsync(actor, targetEmployerId); 


            var newPosterId = dto.ReassignToEmployerId ?? actor.Id;
          


            var newPoster = await _context.EmployerProfiles.FirstOrDefaultAsync(e =>
                    e.Id == newPosterId &&
                    e.Id != target.Id &&
                    e.CompanyId == actor.CompanyId &&
                    e.IsActive&&e.HasAccess)
                ?? throw new InvalidOperationException("Choose a valid active member to take over the jobs.");

            await using var tx = await _context.Database.BeginTransactionAsync();

            var companyId = actor.CompanyId!.Value;
            var newPosterName = await NameOfAsync(newPosterId);
            await _context.JobPosts
                .Where(j => j.CompanyId == companyId && j.PostedByEmployerId == target.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(j => j.PostedByEmployerId, newPoster.Id));

            await _activity.LogAsync(companyId, actor, target, ActivityType.MemberRemoved,
                (a, t) => $"{a} removed {t} from the company. Their jobs were reassigned to {newPosterName}.");


            await tx.CommitAsync();

            return new SuccessResponseDto { Message = "Member removed and their jobs were reassigned." };
        }
        

        public async Task<SuccessResponseDto> LeaveCompanyAsync(RemoveMemberDto dto)
        {
            var me = await _context.EmployerProfiles.FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId)
                ?? throw new KeyNotFoundException("Profile not found.");
            if (me.CompanyId == null) throw new InvalidOperationException("You are not in a company.");
            if (me.Role == EmployerCompanyRole.Owner)
                throw new InvalidOperationException("The Owner cannot leave. Transfer ownership first.");

            var newPoster = await _context.EmployerProfiles.FirstOrDefaultAsync(e =>
                    e.Id == dto.ReassignToEmployerId && e.Id != me.Id &&
                    e.CompanyId == me.CompanyId && e.IsActive && e.HasAccess)
                ?? await _context.EmployerProfiles.FirstAsync(e =>
                    e.CompanyId == me.CompanyId && e.Role == EmployerCompanyRole.Owner);

            await using var tx = await _context.Database.BeginTransactionAsync();
            await _context.JobPosts
                .Where(j => j.CompanyId == me.CompanyId && j.PostedByEmployerId == me.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(j => j.PostedByEmployerId, newPoster.Id));
            var companyId = me.CompanyId!.Value;

            await _activity.LogAsync(companyId, me, null, ActivityType.MemberLeft,
                (a, _) => $"{a} left the company.");
            Unlink(me);
            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return new SuccessResponseDto { Message = "You left the company." };
        }
        public async Task<SuccessResponseDto> TransferOwnershipAsync(int newOwnerEmployerId)
        {
            var acting = await _context.EmployerProfiles.FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId);
            if (acting?.CompanyId == null || acting.Role != EmployerCompanyRole.Owner)
                throw new UnauthorizedAccessException("Only the Owner can transfer ownership.");

            var target = await _context.EmployerProfiles.FirstOrDefaultAsync(e =>
                    e.Id == newOwnerEmployerId && e.CompanyId == acting.CompanyId && e.IsActive && e.HasAccess)
                ?? throw new KeyNotFoundException("Member not found in your company.");

            if (target.Id == acting.Id)
                throw new InvalidOperationException("You are already the Owner.");

            target.Role = EmployerCompanyRole.Owner;
            target.Permissions = EmployerCompanyRole.Owner.DefaultPermissions();
            acting.Role = EmployerCompanyRole.Admin;
            acting.Permissions = EmployerCompanyRole.Admin.DefaultPermissions();

            await _activity.LogAsync(acting.CompanyId!.Value, acting, target, ActivityType.OwnershipTransferred,
               (a, t) => $"{a} transferred ownership to {t}. {a} is now an Admin.");


            await _context.SaveChangesAsync();   // one transaction: never zero or two Owners

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
            var me = await _context.EmployerProfiles.Include(x => x.User)
                 .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId && e.IsActive);

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
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId && e.IsActive && e.HasAccess) ;

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
            await _activity.LogAsync(company.Id, employerProfile, null, ActivityType.CompanyUpdated,
    (a, _) => $"{a} updated the company profile.");

            // Logo
            if (dto.Logo != null)
            {
                if (!dto.Logo.ContentType.StartsWith("image/") || dto.Logo.Length > 2 * 1024 * 1024)
                    throw new ArgumentException("Logo must be an image under 2 MB.");


                // 1. upload the new one first
                var (url, publicId) = await _cloudinaryService.UploadImageAsync(dto.Logo);

                var oldPublicId = company.Logo?.PublicId;
                await _activity.LogAsync(company.Id, employerProfile, null, ActivityType.CompanyLogoChanged,
     (a, _) => $"{a} changed the company logo.");

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
            await LogAsync(company.Id, employerProfile, null, ActivityType.CompanyUpdated,
                            (a, _) => $"{a} updated the company profile.");
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
                    HasAccessed=e.HasAccess,
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

            target.HasAccess = dto.HasAccessed;
            await LogAsync(actor.CompanyId!.Value, actor, target, ActivityType.AccessChanged,
            (a, t) => dto.HasAccessed? $"{a} restored {t}'s access to the company.": $"{a} denied {t}'s access to the company.");
            await _context.SaveChangesAsync();


        }


        public async Task<MyTeamRoleDto> GetMyRoleAsync()
        {
            var me = await _context.EmployerProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UserId == _currentUser.UserId && e.CompanyId != null)
                ?? throw new UnauthorizedAccessException("You are not in a company.");

            var canUse = me.IsActive && me.HasAccess;

            return new MyTeamRoleDto
            {
                Role = me.Role ?? EmployerCompanyRole.Employer,
                Permissions = canUse ? me.Permissions.ToList() : new List<string>(),
                IsActive = me.IsActive,
                HasAccess = me.HasAccess
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
            var removed = target.Permissions.Except(requested).ToList();
            if (added.Any(p => !CanGrant(actor, p)))
                throw new UnauthorizedAccessException("You can't grant permissions you don't have.");

            target.Permissions = requested;
            await _activity.LogAsync(actor.CompanyId!.Value, actor, target, ActivityType.PermissionsChanged, (a, t) =>
            {
                var parts = new List<string>();
                if (added.Any()) parts.Add($"added {string.Join(", ", added)}");
                if (removed.Any()) parts.Add($"removed {string.Join(", ", removed)}");
                return parts.Count == 0
                    ? $"{a} saved {t}'s permissions with no changes."
                    : $"{a} updated {t}'s permissions: {string.Join("; ", parts)}.";
            });
            await _context.SaveChangesAsync();

            return new SuccessResponseDto { Message = "Permissions updated." };
        }

        public async Task<PagedResult<ActivityDto>> GetMyActivityAsync(ActivityFilterDto f)
        {
            var me = await _permissionService.GetCurrentEmployerAsync()
                ?? throw new UnauthorizedAccessException("You are not in a company.");
            if (me.CompanyId == null) throw new InvalidOperationException("You are not in a company.");

            var q = _context.CompanyActivities.AsNoTracking()
                .Where(a => a.CompanyId == me.CompanyId &&
                            (a.ActorEmployerId == me.Id || a.TargetEmployerId == me.Id));

            return await PageAsync(q, f);
        }

        public async Task<PagedResult<ActivityDto>> GetCompanyActivityAsync(ActivityFilterDto f)
        {
            var actor = await GetActorAsync();   // Owner/Admin with ManageEmployerRoles

            var q = _context.CompanyActivities.AsNoTracking()
                .Where(a => a.CompanyId == actor.CompanyId &&
                            !ActivityCategories.Personal.Contains(a.Type));   // hide personal items

            if (f.ActorEmployerId != null) q = q.Where(a => a.ActorEmployerId == f.ActorEmployerId);

            return await PageAsync(q, f);
        }

        private async Task<PagedResult<ActivityDto>> PageAsync(IQueryable<CompanyActivity> q, ActivityFilterDto f)
        {
            if (!string.IsNullOrWhiteSpace(f.Type) && Enum.TryParse<ActivityType>(f.Type, true, out var type))
                q = q.Where(a => a.Type == type);

            if (!string.IsNullOrWhiteSpace(f.Category) &&
                ActivityCategories.Map.TryGetValue(f.Category, out var types))
                q = q.Where(a => types.Contains(a.Type));

            if (f.From != null) q = q.Where(a => a.CreatedAt >= f.From);
            if (f.To != null) q = q.Where(a => a.CreatedAt < f.To.Value.Date.AddDays(1));
            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var s = f.Search.Trim().ToLower();
                q = q.Where(a => a.Message.ToLower().Contains(s));
            }

            var pageSize = Math.Clamp(f.PageSize, 1, 50);
            var page = Math.Max(f.PageNumber, 1);
            var total = await q.CountAsync();

            var rows = await q.OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResult<ActivityDto>
            {
                Items = rows.Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = a.Type.ToString(),
                    Category = ActivityCategories.Of(a.Type),
                    Message = a.Message,
                    ActorEmployerId = a.ActorEmployerId,
                    ActorName = a.ActorName,
                    TargetName = a.TargetName,
                    RefId = a.RefId,
                    CreatedAt = a.CreatedAt
                }).ToList(),
                TotalCount = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

       
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        public Task<PagedResult<ActivityDto>> GetActivityAsync(ActivityFilterDto f)
        {
            throw new NotImplementedException();
        }
    }
}
