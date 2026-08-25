using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared.Dtos.Language;

namespace Presentation
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }
        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLanguageDto dto)
        {
            var result = await _languageService.CreateLanguageAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLanguageDto dto)
        {
            var result = await _languageService.UpdateLanguageAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _languageService.DeleteLanguageAsync(id);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _languageService.GetLanguageByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] LanguageSearchFilterDto filterDto)
        {
            var result = await _languageService.GetAllLanguagesAsync(filterDto);
            return Ok(result);
        }
    }
}