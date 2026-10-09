using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Education
    {
        public int Id { get; set; }
        public int SeekerProfileId { get; set; }
        public string School { get; set; } = "";
        public string Degree { get; set; } = "";
        public string? FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
}
