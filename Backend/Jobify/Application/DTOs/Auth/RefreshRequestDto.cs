using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Auth
{
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = null!;
        public string DeviceId { get; set; } = null!;
    }
}
