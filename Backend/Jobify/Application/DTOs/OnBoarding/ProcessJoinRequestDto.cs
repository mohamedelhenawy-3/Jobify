using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OnBoarding
{
    public class ProcessJoinRequestDto
    {
        public int RequestId { get; set; }
        public bool IsApproved { get; set; }
    }
}
