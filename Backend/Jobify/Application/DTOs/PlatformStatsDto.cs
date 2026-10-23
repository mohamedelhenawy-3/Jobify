using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record PlatformStatsDto(
      int ActiveJobs,
      int Companies,
      int RegisteredUsers,
      int SuccessfulHires
  );
}
