using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.OnBoarding
{
    public class JoinRequestEmployers
    {
        public int Id { get; set; }
        public string? logo { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public JoinRequestStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; }       
        public DateTime? RespondedAt { get; set; }
    }
}
