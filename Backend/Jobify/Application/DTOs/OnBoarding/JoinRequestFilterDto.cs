using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OnBoarding
{
    public class JoinRequestFilterDto
    {
        public JoinRequestStatus? Status { get; set; } = JoinRequestStatus.Pending;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
