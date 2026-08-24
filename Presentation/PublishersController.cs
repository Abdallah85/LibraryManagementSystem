using Microsoft.AspNetCore.Authorization;
using ServicesAbstractions;
using Shared.Dtos.Publisher;

namespace Presentation
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePublisherDto dto)
        {
            var result = await _publisherService.CreatePublisherAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePublisherDto dto)
        {
            var result = await _publisherService.UpdatePublisherAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _publisherService.DeletePublisherAsync(id);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _publisherService.GetPublisherByIdAsync(id);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _publisherService.GetAllPublishersAsync(searchTerm, pageNumber, pageSize);
            return Ok(result);
        }
    }
}