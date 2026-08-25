using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicesAbstractions;
using Shared.Dtos.Book;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllBooks([FromQuery] BookSearchFilterDto filterDto)
        {
            var result = await _bookService.GetAllBooksAsync(filterDto);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            var result = await _bookService.GetBookByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookDto dto)
        {
            var result = await _bookService.CreateBookAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] UpdateBookDto dto)
        {
            var result = await _bookService.UpdateBookAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _bookService.DeleteBookAsync(id);
            return Ok(result);
        }
    }
}
