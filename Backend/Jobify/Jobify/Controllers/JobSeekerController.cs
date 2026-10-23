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
        public async Task<ActionResult<SuccessResponse>> Visibility(UpdateVisibilityDto d) => Ok(await _s.SaveVisibilityAsync(d));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PublicProfileDto>> Get(int id) => Ok(await _s.GetPublicAsync(id));

        [HttpPost("languages")]
        public async Task<ActionResult<ProfileDto>> AddLanguage(LanguageDto dto) => Ok(await _s.SaveLanguageAsync(null, dto));

        [HttpPut("languages/{id}")]
        public async Task<ActionResult<ProfileDto>> UpdateLanguage(int id, LanguageDto dto) => Ok(await _s.SaveLanguageAsync(id, dto));

        [HttpDelete("languages/{id}")]
        public async Task<ActionResult<ProfileDto>> DeleteLanguage(int id) => Ok(await _s.DeleteLanguageAsync(id));

        [HttpPost("certificates")]
        public async Task<ActionResult<ProfileDto>> AddCertificate(CertificateDto dto) => Ok(await _s.SaveCertificateAsync(null, dto));

        [HttpPut("certificates/{id}")]
        public async Task<ActionResult<ProfileDto>> UpdateCertificate(int id, CertificateDto dto) => Ok(await _s.SaveCertificateAsync(id, dto));

        [HttpDelete("certificates/{id}")]
        public async Task<ActionResult<ProfileDto>> DeleteCertificate(int id) => Ok(await _s.DeleteCertificateAsync(id));

        [HttpPost("projects")]
        public async Task<ActionResult<ProfileDto>> AddProject(ProjectDto dto) => Ok(await _s.SaveProjectAsync(null, dto));

        [HttpPut("projects/{id}")]
        public async Task<ActionResult<ProfileDto>> UpdateProject(int id, ProjectDto dto) => Ok(await _s.SaveProjectAsync(id, dto));

        [HttpDelete("projects/{id}")]
        public async Task<ActionResult<ProfileDto>> DeleteProject(int id) => Ok(await _s.DeleteProjectAsync(id));

        [HttpPost("projects/{projectId:int}/images"), Consumes("multipart/form-data")]
        public async Task<ActionResult<ProfileDto>> AddProjectImages(int projectId, [FromForm] List<IFormFile> files)
    => Ok(await _s.AddProjectImagesAsync(projectId, files));

        [HttpDelete("projects/{projectId:int}/images/{imageId:int}")]
        public async Task<ActionResult<ProfileDto>> DeleteProjectImage(int projectId, int imageId)
            => Ok(await _s.DeleteProjectImageAsync(projectId, imageId));

        [HttpPut("projects/{projectId:int}/images/{imageId:int}/cover")]
        public async Task<ActionResult<ProfileDto>> SetProjectCover(int projectId, int imageId)
            => Ok(await _s.SetProjectCoverAsync(projectId, imageId));


    }
}
