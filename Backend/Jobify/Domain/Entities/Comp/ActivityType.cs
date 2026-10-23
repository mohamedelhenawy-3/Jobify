using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Comp
{
    public enum ActivityType
    {
        RoleChanged = 1, PermissionsChanged = 2, AccessChanged = 3, MemberRemoved = 4,
        MemberLeft = 5, OwnershipTransferred = 6, MemberJoined = 7,
        JoinRequestRejected = 8, CompanyUpdated = 9,
        JobCreated = 10, JobUpdated = 11, JobClosed = 12, JobReopened = 13, JobDeleted = 14,
        ApplicationReceived = 20, ApplicantStatusChanged = 21, ApplicantShortlisted = 22,
        ApplicantRejected = 23, ApplicantHired = 24,
        EmployerProfileUpdated = 30,        // personal, shown only in "My activity"
        CompanyLogoChanged = 31
    }
}
