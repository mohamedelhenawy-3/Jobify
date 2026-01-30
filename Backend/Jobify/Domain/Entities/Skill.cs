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

        public string Name { get; set; } = null!;   // "C#", "Angular", "SQL"

        // FK
        public int JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }
    }
}
