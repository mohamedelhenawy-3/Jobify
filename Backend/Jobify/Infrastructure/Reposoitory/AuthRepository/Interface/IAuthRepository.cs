using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reposoitory.AuthRepository.Interface
{
    public interface IAuthRepository
    {
        Task<AppUser> GetUserByEmail(string Email);
        Task<IdentityResult> CreateUserAsync(AppUser user, string password);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
    }
}
