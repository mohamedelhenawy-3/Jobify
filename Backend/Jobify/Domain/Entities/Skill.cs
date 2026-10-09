using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public int SeekerProfileId { get; set; }
        public string Name { get; set; } = "";
    }
}
