using Application.DTOs.SavedJobDto;
using Application.Interfaces.JobsAlert;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobAlertsController : ControllerBase
    {
        private readonly IAlertJobs _service;
        public JobAlertsController(IAlertJobs service) => _service = service;

        [HttpPut("{id:int}/active")]
        public async Task<IActionResult> SetActive(int id, SetAlertActiveDto dto)=> Ok(await _service.SetActiveAsync(id, dto));

        [HttpPost]
        public async Task<IActionResult> Create(CreateJobAlertDto dto) => Ok(await _service.CreateAsync(dto));

        [HttpGet]
        public async Task<IActionResult> Mine() => Ok(await _service.GetMineAsync());

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
    }
}
