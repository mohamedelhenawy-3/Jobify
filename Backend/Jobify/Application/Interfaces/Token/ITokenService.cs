using Application.DTOs.Auth.AuthResponse;
using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Token
{
    public interface ITokenService
    {
        Task<LoginResponseDto> GenerateToken(AppUser user, string deviceId);
        Task<LoginResponseDto?> RefreshToken(string refreshTokenValue , string deviceId);
        Task <SuccessResponseDto> Logout(string refreshtoken, string deviceId);

    }
}
