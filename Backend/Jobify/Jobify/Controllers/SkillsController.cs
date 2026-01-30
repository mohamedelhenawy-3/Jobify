using Application.DTOs.Skills;
using Application.Interfaces.Skills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsService _skillsService;

        public SkillsController(ISkillsService skillsService)
        {
            _skillsService = skillsService;
        }

        // ✅ ADD SKILLS (from profile)
        [HttpPost]
        public async Task<IActionResult> AddSkills([FromBody] AddSkillsDto dto)
        {
            if (dto == null || dto.Skills == null || !dto.Skills.Any())
                return BadRequest("Skills list cannot be empty");

            var result = await _skillsService.AddSkillsAsync(dto);
            return Ok(result);
        }

        // ✅ GET MY SKILLS
        [HttpGet("my")]
        public async Task<IActionResult> GetMySkills()
        {
            var skills = await _skillsService.AllSkills();
            return Ok(skills);
        }

        // ✅ REMOVE SKILL
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> RemoveSkill(int id)
        {
            var result = await _skillsService.RemoveSkill(id);
            return Ok(result);
        }
    }
}
