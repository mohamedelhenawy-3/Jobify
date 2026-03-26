using Application.DTOs.Auth.AuthResponse;
using Application.DTOs.Employer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Employer
{
    public interface IEmployer
    {
         Task<EmployerProfileResponseDto> MyProfile();
        Task<SuccessResponseDto> UpdateAsync(UpdateEmployerProfile model);

    }
}
