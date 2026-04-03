using Application.SearchFeature.Dtos;
using Application.SearchFeature.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] GlobalSearchDto dto)
        {
            var result = await _searchService.GlobalSearchAsync(dto);
            return Ok(result);
        } 
    }
}
