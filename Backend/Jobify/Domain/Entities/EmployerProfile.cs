using Domain.Entities.LooksUp.EmployerPermisions;
using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class EmployerProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string? Bio { get; set; }
        public string? Position { get; set; }
        public bool IsPublic { get; set; } = true;
        public EmployerCompanyRole? Role { get; set; }
        public List<string> Permissions { get; set; } = new();

        public bool IsActive { get; set; } = true;
        public bool HasAccess { get; set; } = true;


        public DateTime? JoinedCompanyAt { get; set; }

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        public ProfileImage? ProfileImage { get; set; }

      
        
    }
}
