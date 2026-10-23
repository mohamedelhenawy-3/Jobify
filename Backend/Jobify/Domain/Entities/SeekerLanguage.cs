using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SeekerLanguage
    {
        public int Id { get; set; }
        public int JobSeekerProfileId { get; set; }
        public string Name { get; set; } = "";
        public string Level { get; set; } = "";
    }
}
