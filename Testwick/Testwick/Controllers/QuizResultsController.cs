using Microsoft.AspNetCore.Mvc;
using Testwick.DTOs;
using Testwick.Services;

namespace Testwick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizResultsController(IQuizResultsService quizResultsService) : Controller
    {
        private readonly IQuizResultsService _quizResultsService = quizResultsService;

        // Submit answers for a quiz
        [HttpPost]
        public async Task<IActionResult> Create(CreateQuizResultDto dto)
        {
            try
            {
                var result = await _quizResultsService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _quizResultsService.GetByIdAsync(id);
            return result is null? NotFound() : Ok(result);
        }

        // List all results for a quiz
        [HttpGet("quiz/{quizId:int}")]
        public async Task<IActionResult> GetByQuizId(int quizId)
        {
            return Ok(await _quizResultsService.GetByQuizIdAsync(quizId));
        }
    }
}
