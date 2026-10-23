using Domain.Entities;
using Domain.Entities.Comp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Logger
{
    public interface IActivityLogger
    {
        Task LogAsync(int companyId, EmployerProfile? actor, EmployerProfile? target,
                ActivityType type, Func<string, string?, string> message,
                int? refId = null, string? actorNameOverride = null);
    }
}
