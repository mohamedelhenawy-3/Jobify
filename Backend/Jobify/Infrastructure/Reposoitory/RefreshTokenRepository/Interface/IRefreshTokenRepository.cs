using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Reposoitory.RefreshTokenRepository.Interface
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetAllByUserAsync(string userId);
        Task<RefreshToken?> GetByTokenAndDeviceAsync(string token, string deviceId);

        Task AddRefreshToken(RefreshToken refreshtoken);
        Task UpdateAsync(RefreshToken token);
        Task RevokeAllByUserAsync(string userId);

        Task RevokeAsync(RefreshToken token);
    }
}
