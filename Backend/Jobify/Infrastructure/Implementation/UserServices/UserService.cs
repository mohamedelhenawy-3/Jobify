using Application.DTOs.Comapny;
using Application.DTOs.Education;
using Application.DTOs.Experience;
using Application.DTOs.Skills;
using Application.DTOs.User;
using Application.Interfaces.User;
using Infrastructure.Implementation.AuthService;
using Infrastructure.Implementation.CLoudinary;
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

        public UserService(
              AppDBContext context,
              CloudinaryService cloudinaryService,
              IHttpContextAccessor httpContextAccessor,
               SupabaseService supabaseservice

              )
        {
            _db = context;
            _cloudinaryService = cloudinaryService;
            _httpaccessor = httpContextAccessor;
            _supabaseservice = supabaseservice;
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
