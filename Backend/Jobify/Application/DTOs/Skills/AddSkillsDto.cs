using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Skills
{
    public class AddSkillsDto
    {
        public List<string> Skills { get; set; } = new();
    }
}
