using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Comapny;
using Application.DTOs.Education;
using Application.DTOs.Experience;
using Application.DTOs.Skills;
using Application.DTOs.User;
using Application.Interfaces.User;
using Infrastructure.CurrentUserService;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.CLoudinary;
using Infrastructure.Implementation.CompanyMembersService.CurrentUserService;
using Infrastructure.Implementation.FileUploadService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Infrastructure.Implementation.UserServices
{
    public class UserService :IUser
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;
        private readonly CloudinaryService _cloudinaryService;
        private readonly SupabaseService _supabaseservice;
        private ICurrentUser _currentuser;

        public UserService(
              AppDBContext context,
              CloudinaryService cloudinaryService,
              IHttpContextAccessor httpContextAccessor,
               SupabaseService supabaseservice,
               ICurrentUser c

              )
        {
            _db = context;
            _cloudinaryService = cloudinaryService;
            _httpaccessor = httpContextAccessor;
            _supabaseservice = supabaseservice;
            _currentuser = c;
        }

        public async Task<UserSummaryDto?> GetMySummaryAsync()
        {
            var userId = _currentuser.UserId;
            if (string.IsNullOrEmpty(userId)) return null;

            return await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserSummaryDto
                {
                    FullName = u.FullName,
                    PhotoUrl =
                        u.EmployerProfile != null && u.EmployerProfile.ProfileImage != null
                            ? u.EmployerProfile.ProfileImage.ImageUrl
                        : u.JobSeeker != null && u.JobSeeker.ProfileImage != null
                            ? u.JobSeeker.ProfileImage.ImageUrl
                        : null
                })
                .FirstOrDefaultAsync();
        }


        public Task<GetUserProfile?> GetUserProfileAsync(string userId)
        {
            throw new NotImplementedException();
        }

        private string? GetCurrentUserId()
        {
            var user = _httpaccessor.HttpContext?.User;
            return user?.FindFirstValue(ClaimTypes.NameIdentifier);
        }


    }
}
