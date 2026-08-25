using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using ServicesAbstractions;
using Shared.Dtos.Language;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanguagesController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLanguageDto dto)
        {
            dto.CreatedBy = User.GetUserId();
            var result = await _languageService.CreateLanguageAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLanguageDto dto)
        {
            dto.UpdatedBy = User.GetUserId();
            var result = await _languageService.UpdateLanguageAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _languageService.DeleteLanguageAsync(id, User.GetUserId());
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _languageService.GetLanguageByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] LanguageSearchFilterDto filterDto)
        {
            var result = await _languageService.GetAllLanguagesAsync(filterDto);
            return Ok(result);
        }
    }
}