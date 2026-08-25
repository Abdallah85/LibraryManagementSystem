using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using ServicesAbstractions;
using Shared.Dtos.Author;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllAuthors([FromQuery] AuthorSearchFilterDto filterDto)
        {
            var result = await _authorService.GetAllAuthorsAsync(filterDto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetAuthorById(int id)
        {
            var result = await _authorService.GetAuthorByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> CreateAuthor([FromBody] CreateAuthorDto dto)
        {
            var createdBy = User.GetUserId();
            dto.CreatedBy = createdBy;
            var result = await _authorService.CreateAuthorAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] UpdateAuthorDto dto)
        {
            var updatedBy = User.GetUserId();
            dto.UpdatedBy = updatedBy;
            var result = await _authorService.UpdateAuthorAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var deletedBy = User.GetUserId();
            var result = await _authorService.DeleteAuthorAsync(id, deletedBy!);
            return Ok(result);
        }
    }
}