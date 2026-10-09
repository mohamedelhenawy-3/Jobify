using Application.DTOs.JobSeeker;
using Application.DTOs.JobSeeker.JobSeekers;
using Application.DTOs.JobSeeker.JobSeekers.Updates;
using Application.DTOs.SuccefullOp;
using Application.Interfaces.JobSeeker;
using Infrastructure.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [Authorize]                                   // add this, you import it but don't use it
    [ApiController]
    public class JobSeekerController : ControllerBase
    {
        private readonly IJobSeekerService _s;
        public JobSeekerController(IJobSeekerService s) => _s = s;

        [HttpGet("me")] public async Task<ActionResult<ProfileDto>> Me() => Ok(await _s.GetMyAsync());
        [HttpPut("basic")] public async Task<ActionResult<ProfileDto>> Basic(UpdateBasicDto d) => Ok(await _s.UpdateBasicAsync(d));
        [HttpPut("about")] public async Task<ActionResult<ProfileDto>> About(UpdateAboutDto d) => Ok(await _s.UpdateAboutAsync(d));
        [HttpPost("experiences")] public async Task<ActionResult<ProfileDto>> AddExp(ExperienceDto d) => Ok(await _s.SaveExperienceAsync(null, d));
        [HttpPut("experiences/{id:int}")] public async Task<ActionResult<ProfileDto>> EditExp(int id, ExperienceDto d) => Ok(await _s.SaveExperienceAsync(id, d));
        [HttpDelete("experiences/{id:int}")] public async Task<ActionResult<ProfileDto>> DelExp(int id) => Ok(await _s.DeleteExperienceAsync(id));

        [HttpPost("educations")] public async Task<ActionResult<ProfileDto>> AddEdu(EducationDto d) => Ok(await _s.SaveEducationAsync(null, d));
        [HttpPut("educations/{id:int}")] public async Task<ActionResult<ProfileDto>> EditEdu(int id, EducationDto d) => Ok(await _s.SaveEducationAsync(id, d));
        [HttpDelete("educations/{id:int}")] public async Task<ActionResult<ProfileDto>> DelEdu(int id) => Ok(await _s.DeleteEducationAsync(id));

        [HttpPut("skills")] public async Task<ActionResult<ProfileDto>> Skills(UpdateSkillsDto d) => Ok(await _s.SaveSkillsAsync(d));
        [HttpPut("preferences")] public async Task<ActionResult<ProfileDto>> Prefs(UpdatePreferencesDto d) => Ok(await _s.SavePreferencesAsync(d));
        [HttpPost("photo")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProfileDto>> Photo([FromForm] UpdateProfileImage dto) => Ok(await _s.UpdateProfileImge(dto));
        [HttpPost("cv")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ProfileDto>> UploadCv([FromForm] UploadMyResume dto)  => Ok(await _s.UploadCv(dto));
        [HttpDelete("cv")]
        public async Task<ActionResult<ProfileDto>> RemoveCv() => Ok(await _s.RemoveCv());

        [HttpPut("visibility")]
        public async Task<ActionResult<ProfileDto>> Visibility(UpdateVisibilityDto d)  => Ok(await _s.SaveVisibilityAsync(d));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PublicProfileDto>> Get(int id) => Ok(await _s.GetPublicAsync(id));

    }
}
