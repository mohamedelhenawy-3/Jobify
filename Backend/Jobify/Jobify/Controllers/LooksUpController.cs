using Infrastructure.Implementation.AuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LooksUpController : ControllerBase
    {
        private readonly AppDBContext _db;
        public LooksUpController(AppDBContext db)
        {
            _db = db;
        }

        [HttpGet("Industries")]
        public async Task<IActionResult> GetIndustries()
        {
            var industries = await _db.JobIndustries
                .Select(i => new { i.Id, i.IndustryName })
                .ToListAsync();

            return Ok(industries);
        }

        [HttpGet("EmploymentTypes")]
        public async Task<IActionResult> GetEmploymentTypes()
        {
            var types = await _db.EmploymentTypes
                .Select(t => new { t.Id, t.TypeName })
                .ToListAsync();

            return Ok(types);
        }

       
        [HttpGet("WorkModes")]
        public async Task<IActionResult> GetWorkModes()
        {
            var modes = await _db.WorkModes
                .Select(w => new { w.Id, w.Name })
                .ToListAsync();

            return Ok(modes);
        }
        //[HttpGet("Statuses")]
        //public async Task<IActionResult> GetStatus()
        //{
        //    var modes = await _db.JobStatuses
        //        .Select(w => new { w.Id, w.StatusName })
        //        .ToListAsync();

        //    return Ok(modes);
        //}


     
        [HttpGet("All")]
        public async Task<IActionResult> GetAllLookups()
        {
            var industries = await _db.JobIndustries
                   .Select(i => new { i.Id, i.IndustryName })
                   .ToListAsync();

            var types = await _db.EmploymentTypes
                .Select(t => new { t.Id, t.TypeName })
                .ToListAsync();

            var modes = await _db.WorkModes
                .Select(w => new { w.Id, w.Name })
                .ToListAsync();

            return Ok(new
            {
                industries,
                employmentTypes = types,
                workModes = modes
            });
        }
    }
}
