using Application.DTOs.Auth;
using Application.DTOs.Auth.AuthResponse;
using Application.Interfaces.Auth;
using Application.Interfaces.Token;
using Domain.Entities;
using Domain.Identity;
using Infrastructure.Reposoitory.AuthRepository.Interface;
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
        private readonly IAuthRepository _authrepo;
        private readonly ITokenService _jwtService;



        public AuthService(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDBContext context
            , IJobSeeker jobSeekerRepo, IEmployeer employerRepo,
            IAuthRepository authrepo,
            ITokenService jwtService,
            IConfiguration config)
        {
            _jobSeekerRepo = jobSeekerRepo;
            _employerRepo = employerRepo;
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _config = config;
            _authrepo = authrepo;
            _jwtService = jwtService;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _authrepo.GetUserByEmail(dto.Email);
            if (user == null)
                throw new Exception("Invalid credentials");
            var validPassword = await _authrepo.CheckPasswordAsync(user, dto.Password);
            if (!validPassword)
                throw new Exception("Invalid credentials");


            var token = await _jwtService.GenerateToken(user, dto.DeviceId);
            


            return new LoginResponseDto
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken
            };
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
                };

                await _jobSeekerRepo.AddAsync(seeker);
                await _jobSeekerRepo.SaveChangesAsync();
            }
            else if (dto.Role == "Employer")
            {
          

                var employer = new EmployerProfile
                {
                    UserId = user.Id,
                   
                };

                _context.EmployerProfiles.Add(employer);
                await _context.SaveChangesAsync(); // employer.Id is now real

                //var company = new Company
                //{
                //    CompanyName = dto.CompanyName,
                //    CompanyWebsite = dto.CompanyWebsite,
                //    Industry = dto.Industry,
                //    Location = dto.CompanyLocation,
                //    EmployerProfileId = employer.Id,
                //    CreatedAt = DateTime.UtcNow
                //};

                //_context.Companies.Add(company);
                //await _context.SaveChangesAsync();
            }
            return new SuccessResponseDto
            {
                Message = "User registered successfully"
            };


        }

      
    }

     
    
}

