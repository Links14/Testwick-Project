using Microsoft.AspNetCore.Mvc;
using Testwick.DTOs;
using Testwick.Services;

namespace Testwick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController(ITopicService topicService) : ControllerBase
    {
        private readonly ITopicService _topicService = topicService;

        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _topicService.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var topic = await _topicService.GetByIdAsync(id);
            return topic is null ? NotFound() : Ok(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTopicDto dto)
        {
            var created = await _topicService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CreateTopicDto dto)
        {
            var success = await _topicService.UpdateAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _topicService.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}