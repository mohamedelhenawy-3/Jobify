using Application.DTOs.Auth;
using Application.DTOs.Auth.AuthResponse;
using Application.Interfaces.Auth;

using Domain.Entities;
using Domain.Identity;
using Infrastructure.DBContext;
using Infrastructure.Reposoitory.UserRepository.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDBContext _context;
        private readonly IConfiguration _config;
        private readonly IJobSeeker _jobSeekerRepo;
        private readonly IEmployeer _employerRepo;



        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDBContext context
            , IJobSeeker jobSeekerRepo, IEmployeer employerRepo,

            IConfiguration config)
        {
            _jobSeekerRepo = jobSeekerRepo;
            _employerRepo = employerRepo;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _config = config;
        }
        public async Task<SuccessResponseDto> RegisterAsync(RegisterDto dto)
        {
            if (!await _roleManager.RoleExistsAsync(dto.Role))
                throw new Exception("Role does not exist");

            var user = new AppUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            // 2️⃣ Assign role
            await _userManager.AddToRoleAsync(user, dto.Role);
            if (dto.Role == "JobSeeker")
            {
                var seeker = new JobSeeker
                {
                    UserId = user.Id,
                    PhoneNumber = dto.PhoneNumber
                };

                await _jobSeekerRepo.AddAsync(seeker);
                await _jobSeekerRepo.SaveChangesAsync();
            }
            else if (dto.Role == "Employer")
            {
                if (string.IsNullOrWhiteSpace(dto.CompanyName))
                    throw new Exception("Company name is required");

                var employer = new EmployerProfile
                {
                    UserId = user.Id,
                    CompanyName = dto.CompanyName!,
                    CompanyWebsite = dto.CompanyWebsite
                };

                await _employerRepo.AddAsync(employer);
                await _employerRepo.SaveChangesAsync();
            }
            return new SuccessResponseDto
            {
                Message = "User registered successfully"
            };


        }
        
    }

     
    
}

