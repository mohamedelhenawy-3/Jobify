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
      
        public ProfileImage? ProfileImage { get; set; }
        public Company? Company { get; set; }

      
        
    }
}
