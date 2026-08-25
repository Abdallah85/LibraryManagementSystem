using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using ServicesAbstractions;
using Shared.Dtos.Category;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
        {
            dto.CreatedBy = User.GetUserId();
            var result = await _categoryService.CreateCategoryAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            dto.UpdatedBy = User.GetUserId();
            var result = await _categoryService.UpdateCategoryAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedBy = User.GetUserId();
            var result = await _categoryService.DeleteCategoryAsync(id, deletedBy!);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] CategorySearchFilterDto filterDto)
        {
            var result = await _categoryService.GetAllCategoriesAsync(filterDto);
            return Ok(result);
        }
    }
}