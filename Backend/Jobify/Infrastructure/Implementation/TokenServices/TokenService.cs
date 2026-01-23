using Application.DTOs.Auth.AuthResponse;
using Application.Interfaces.Token;
using Azure.Core;
using Domain.Entities;
using Domain.Identity;
using Infrastructure.Reposoitory.RefreshTokenRepository.Interface;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.TokenServices
{
    public class TokenService : ITokenService
    {
        private readonly JwtGenerator _jwt;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepo;



        public TokenService(
          IConfiguration configuration,
          UserManager<AppUser> userManager,
          IRefreshTokenRepository refreshTokenRepo, JwtGenerator Jwt)

        {
            _configuration = configuration;
            _userManager = userManager;
            _refreshTokenRepo = refreshTokenRepo;
            _jwt = Jwt;
        }
        public async Task<LoginResponseDto> GenerateToken(AppUser user, string deviceId)
        {
            var accesstoken = await _jwt.GenerateTokenAsync(user);
            var refreshtoken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRandomToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(
                  Convert.ToDouble(_configuration["JwtSettings:RefreshTokenDays"])
                   ),
                DeviceId = deviceId
            };

            await _refreshTokenRepo.AddRefreshToken(refreshtoken);
            return new LoginResponseDto
            {
                AccessToken = accesstoken,
                RefreshToken = refreshtoken.Token
            };
        }

        public async Task<SuccessResponseDto> Logout(string refreshtoken,string deviceId)
        {

            var reftoken = await _refreshTokenRepo.GetByTokenAndDeviceAsync(refreshtoken, deviceId);

            if (reftoken != null)
            {
                reftoken.IsRevoked = true;
                await _refreshTokenRepo.UpdateAsync(reftoken);
            }
            return new SuccessResponseDto { Message = "Logout successful" };
        }

        public async  Task<LoginResponseDto?> RefreshToken(string refreshTokenValue, string deviceId)
        {
            var existingToken = await _refreshTokenRepo.GetByTokenAndDeviceAsync(refreshTokenValue,deviceId);
            if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiresAt < DateTime.UtcNow)
                return null;
            await _refreshTokenRepo.RevokeAsync(existingToken);

            var user = await _userManager.FindByIdAsync(existingToken.UserId);
            if (user == null) return null;

            var newAccessToken = await _jwt.GenerateTokenAsync(user);

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                DeviceId = existingToken.DeviceId,
                Token = GenerateRandomToken(),
                ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenDays"]))
            };
            await _refreshTokenRepo.AddRefreshToken(newRefreshToken);
            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };






        }

        private string GenerateRandomToken(int size = 64)
        {
            var randomNumber = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    }
}
