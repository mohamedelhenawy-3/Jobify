using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Comp
{
    public class CompanyActivity
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public ActivityType Type { get; set; }

        public int? ActorEmployerId { get; set; }
        public string ActorName { get; set; } = "";
        public int? TargetEmployerId { get; set; }
        public string? TargetName { get; set; }
        public int? RefId { get; set; }   // NEW: job id or application id, for the UI link

        public ActivityEntity EntityType { get; set; } = ActivityEntity.None;
        public int? EntityId { get; set; }
        public string? EntityName { get; set; }  
        public string? MetaJson { get; set; }    

        public string Message { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }


}
