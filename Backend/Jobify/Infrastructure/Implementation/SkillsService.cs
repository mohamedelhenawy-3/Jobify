using Application.DTOs.Skills;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.Skills;
using Domain.Entities;
using Infrastructure.DBContext;
using Infrastructure.Implementation.CLoudinary;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Infrastructure.Implementation
{
    public class SkillsService : ISkillsService
    {
        private readonly AppDBContext _db;
        private readonly IHttpContextAccessor _httpaccessor;

        public SkillsService(
            AppDBContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _db = context;
            _httpaccessor = httpContextAccessor;
        }

        private string? GetCurrentUserId()
        {
            return _httpaccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // ✅ ADD SKILLS
        public async Task<SuccessResponse> AddSkillsAsync(AddSkillsDto skillsdto)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            var jobSeeker = await _db.JobSeekers
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (jobSeeker == null)
                throw new Exception("Job seeker profile not found");

            var existingSkills = await _db.Skills
                .Where(s => s.JobSeekerId == jobSeeker.Id)
                .Select(s => s.Name.ToLower())
                .ToListAsync();

            var newSkills = skillsdto.Skills
                .Where(s => !existingSkills.Contains(s.ToLower()))
                .Select(s => new Skill
                {
                    Name = s.Trim(),
                    JobSeekerId = jobSeeker.Id
                })
                .ToList();

            if (!newSkills.Any())
            {
                return new SuccessResponse
                {
                    Message = "No new skills to add"
                };
            }

            await _db.Skills.AddRangeAsync(newSkills);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
                Message = "Skills added successfully"
            };
        }

        // ✅ GET MY SKILLS
        public async Task<IEnumerable<MySkills>> AllSkills()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            return await _db.Skills
                .Where(s => s.JobSeeker.UserId == userId)
                .Select(s => new MySkills
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();
        }

        // ✅ REMOVE SKILL
        public async Task<SuccessResponse> RemoveSkill(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException();

            var skill = await _db.Skills
                .Include(s => s.JobSeeker)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (skill == null)
                throw new Exception("Skill not found");

            if (skill.JobSeeker.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete this skill");

            _db.Skills.Remove(skill);
            await _db.SaveChangesAsync();

            return new SuccessResponse
            {
            
                Message = "Skill removed successfully"
            };
        }
    }
}
