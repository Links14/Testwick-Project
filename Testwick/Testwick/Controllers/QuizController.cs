using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Testwick.Data;
using Testwick.DTOs;
using Testwick.Models;
using Testwick.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Testwick.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController(IQuizService quizService) : ControllerBase
    {
        private readonly IQuizService _quizService = quizService;


        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var quizzes = await _quizService.GetAllAsync();
            return quizzes is null|| !quizzes.Any() ? NotFound() : Ok(quizzes);

        }

        [HttpGet("ByName")]
        public async Task<IActionResult> GetByNameAsync(string title)
        {
            var quizzes = await _quizService.GetAllAsync();
            var named = quizzes.Where(q => q.Title.Equals(title, StringComparison.CurrentCultureIgnoreCase));
            return named is null || !named.Any() ? NotFound() : Ok(named);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var quiz = await _quizService.GetByIdAsync(id);
            return quiz is null ? NotFound() : Ok(quiz);
        }

        // returns QuizCreatedDto - the only token including response
        [HttpPost]
        public async Task<IActionResult> Create(CreateQuizDto dto)
        {
            var created = await _quizService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        // admin gated
        [HttpGet("admin/{adminToken:Guid}")]
        public async Task<IActionResult> GetByAdminToken(Guid adminToken)
        {
            var quiz = await _quizService.GetByAdminTokenAsync(adminToken);
            return quiz is null ? NotFound() : Ok(quiz);
        }

        [HttpPut("admin/{adminToken:Guid}")]
        public async Task<IActionResult> Update(Guid adminToken, CreateQuizDto dto)
        {
            var success = await _quizService.UpdateByAdminTokenAsync(adminToken, dto);
            return success ? NoContent() : NotFound();
        }

        [HttpPut("admin/{adminToken:Guid}/AddQuestion/")]
        public async Task<IActionResult> AddQuestion(Guid adminToken, int questionId)
        {
            try
            {
                var updated = await _quizService.AddExistingQuestionByAdminTokenAsync(adminToken, questionId);
                return updated is null ? NotFound() : Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            } 
        }

        [HttpPut("admin/{adminToken:Guid}/RemoveQuestion/")]
        public async Task<IActionResult> RemoveQuestion(Guid adminToken, int questionId)
        {
            var updated = await _quizService.RemoveExistingQuestionByAdminTokenAsync(adminToken, questionId);
            return updated is null ? NotFound() : Ok(updated);
        }

        // contributor gated

        // Load the quiz so the contributor can see what's already in it
        [HttpGet("contribute/{contributorToken:guid}")]
        public async Task<IActionResult> GetByContributorToken(Guid contributorToken)
        {
            var quiz = await _quizService.GetByContributorTokenAsync(contributorToken);
            return quiz is null ? NotFound() : Ok(quiz);
        }

        // submit a new question
        [HttpPost("contribute/{contributorToken:guid}")]
        public async Task<IActionResult> ContributeQuestion(Guid contributorToken, CreateQuestionDto dto)
        {
            var update = await _quizService.ContributeQuestionAsync(contributorToken, dto);
            return update is null ? NotFound() : Ok(update);
        }
    }
}
