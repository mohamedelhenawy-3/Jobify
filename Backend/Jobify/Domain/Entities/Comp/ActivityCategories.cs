using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Comp
{
    public class ActivityCategories
    {
        public static readonly Dictionary<string, ActivityType[]> Map =
      new(StringComparer.OrdinalIgnoreCase)
      {
          ["Team"] = new[] { ActivityType.RoleChanged, ActivityType.PermissionsChanged,
            ActivityType.AccessChanged, ActivityType.MemberRemoved, ActivityType.MemberLeft,
            ActivityType.OwnershipTransferred, ActivityType.MemberJoined,
            ActivityType.JoinRequestRejected },
          ["Company"] = new[] { ActivityType.CompanyUpdated, ActivityType.CompanyLogoChanged },
          ["Jobs"] = new[] { ActivityType.JobCreated, ActivityType.JobUpdated, ActivityType.JobClosed,
            ActivityType.JobReopened, ActivityType.JobDeleted },
          ["Applicants"] = new[] { ActivityType.ApplicationReceived, ActivityType.ApplicantStatusChanged,
            ActivityType.ApplicantShortlisted, ActivityType.ApplicantRejected, ActivityType.ApplicantHired },
          ["Profile"] = new[] { ActivityType.EmployerProfileUpdated },
      };

        public static readonly ActivityType[] Personal = { ActivityType.EmployerProfileUpdated };

        public static string Of(ActivityType t) =>
            Map.First(kv => kv.Value.Contains(t)).Key;
    }
}
