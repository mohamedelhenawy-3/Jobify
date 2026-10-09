using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.LooksUp.EmployerPermisions
{
    public static  class EmployerPermissions
    {
        public const string PostJobs = "PostJobs";
        public const string ManageApplicants = "ManageApplicants";
        public const string ViewAnalytics = "ViewAnalytics";
        public const string ManageCompanyProfile = "ManageCompanyProfile";
        public const string ManageEmployerRoles = "ManageEmployerRoles";
        public const string ApproveJoinRequests = "ApproveJoinRequests";

        public static readonly IReadOnlyList<string> All = new[]
        {
            PostJobs, ManageApplicants, ViewAnalytics,
            ManageCompanyProfile, ManageEmployerRoles, ApproveJoinRequests
        };
    }
}
