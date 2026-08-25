using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using ServicesAbstractions;
using Shared.Dtos.Publisher;

namespace Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublishersController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePublisherDto dto)
        {
            dto.CreatedBy = User.GetUserId();
            var result = await _publisherService.CreatePublisherAsync(dto);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePublisherDto dto)
        {
            dto.UpdatedBy = User.GetUserId();
            var result = await _publisherService.UpdatePublisherAsync(id, dto);
            return Ok(result);
        }

        [Authorize(Roles = "Administrator,Librarian")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _publisherService.DeletePublisherAsync(id, User.GetUserId());
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _publisherService.GetPublisherByIdAsync(id);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PublisherFilterDto filterDto)
        {
            var result = await _publisherService.GetAllPublishersAsync(filterDto);
            return Ok(result);
        }
    }
}