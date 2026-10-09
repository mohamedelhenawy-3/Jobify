using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Experience
    {
        public int Id { get; set; }
        public int SeekerProfileId { get; set; }
        public string Title { get; set; } = "";
        public string Company { get; set; } = "";
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
    }
}
