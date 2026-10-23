using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public string Category { get; set; } = "";     // NEW, for icon and color
        public string Message { get; set; } = "";
        public int? ActorEmployerId { get; set; }      // NEW
        public string ActorName { get; set; } = "";
        public string? TargetName { get; set; }
        public int? RefId { get; set; }                // NEW
        public DateTime CreatedAt { get; set; }
    }
}
