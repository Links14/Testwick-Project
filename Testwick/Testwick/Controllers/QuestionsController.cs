using Microsoft.AspNetCore.Mvc;
using Testwick.DTOs;
using Testwick.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Testwick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController(IQuestionService questionService) : ControllerBase
    {
        private readonly IQuestionService _questionService = questionService;

        /// <summary>
        /// Get all questions without showing the answer or Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _questionService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var question = await _questionService.GetByIdAsync(id);
            if (question is null) return NotFound();
            return Ok(question);
        }

        [HttpGet("topic/{topic}")]
        public async Task<IActionResult> GetByTopic(string topic)
        {
            return Ok(await _questionService.GetByTopicAsync(topic));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateQuestionDto dto)
        {
            var created = await _questionService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // Do not allow access to Update or Delete functionality
        // Hard deletions should be done by a serverside administrator through SSMS
    }
}
