using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.LooksUp.EmployerPermisions
{
    public static class EmployerCompanyRoleExtensions
    {
        public static List<string> DefaultPermissions(this EmployerCompanyRole role)
        {
            switch (role)
            {
                case EmployerCompanyRole.Owner:
                    return EmployerPermissions.All.ToList();

                case EmployerCompanyRole.Admin:
                    return EmployerPermissions.All
                        .Where(p => p != EmployerPermissions.ManageCompanyProfile)
                        .ToList();

                case EmployerCompanyRole.Recruiter:
                    return new List<string>
                    {
                        EmployerPermissions.PostJobs,
                        EmployerPermissions.ManageApplicants,
                        EmployerPermissions.ViewAnalytics
                    };

                case EmployerCompanyRole.Employer:
                    return new List<string>
                    {
                        EmployerPermissions.PostJobs,
                        EmployerPermissions.ManageApplicants
                    };

                default:
                    return new List<string>();
            }
        }
    }
}
