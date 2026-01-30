using Application.DTOs.Skills;
using Application.DTOs.SuccefullOp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Skills
{
    public interface ISkillsService
    {
        Task<SuccessResponse> AddSkillsAsync(AddSkillsDto skillsdto);
        Task<SuccessResponse> RemoveSkill(int id);
        Task<IEnumerable<MySkills>> AllSkills();

    }
}
