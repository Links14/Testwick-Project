using Microsoft.EntityFrameworkCore;
using Testwick.Data;
using Testwick.DTOs;
using Testwick.Models;

namespace Testwick.Services
{
    public class QuizService(AppDbContext db, IQuestionService questionService) : IQuizService
    {
        private readonly AppDbContext _db = db;
        private readonly IQuestionService _questionService = questionService;

        // Get
        public async Task<IEnumerable<QuizDto>> GetAllAsync()
        {
            return await _db.Quizzes
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                        .ThenInclude(q => q.Choices)
                .Select(q => MapToDto(q))
                .ToListAsync();
        }
        public async Task<QuizDto?> GetByIdAsync(int id)
        {
            var quiz = await LoadQuizWithQuestions(q => q.Id == id);
            return quiz is null ? null : MapToDto(quiz);
        }
        public async Task<QuizDto?> GetByAdminTokenAsync(Guid adminToken)
        {
            var quiz = await LoadQuizWithQuestions(q => q.AdminToken == adminToken);
            return quiz is null ? null : MapToDto(quiz);
        }
        public async Task<QuizDto?> GetByContributorTokenAsync(Guid contributorToken)
        {
            var quiz = await LoadQuizWithQuestions(q => q.ContributorToken == contributorToken);
            return quiz is null ? null : MapToDto(quiz);
        }


        // Post
        public async Task<QuizCreatedDto> CreateAsync(CreateQuizDto dto)
        {
            var quiz = new Quiz { Title = dto.Title };
            _db.Add(quiz);
            await _db.SaveChangesAsync();

            _db.QuizQuestions.AddRange(dto.QuestionIds.Select((questionId, i) => new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = questionId,
                Position = i
            }));

            await _db.SaveChangesAsync();

            var created = await LoadQuizWithQuestions(q => q.Id == quiz.Id);
            return MapToCreatedDto(created!);
        }

        public async Task<bool> UpdateByAdminTokenAsync(Guid adminToken, CreateQuizDto dto)
        {
            var quiz = await _db.Quizzes
                .Where(q => q.AdminToken == adminToken)
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync();

            if (quiz is null) return false;

            quiz.Title = dto.Title;
            _db.QuizQuestions.RemoveRange(quiz.QuizQuestions);
            _db.QuizQuestions.AddRange(dto.QuestionIds.Select((questionId, i) => new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = questionId,
                Position = i
            }));

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<QuizDto?> AddExistingQuestionByAdminTokenAsync(Guid adminToken, int questionId)
        {
            var quiz = await _db.Quizzes
                .Where(q => q.AdminToken == adminToken)
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync();

            if (quiz is null) return null;

            var questionExists = await _db.Questions.AnyAsync(q => q.Id == questionId);
            if (!questionExists) return null;

            bool alreadyAdded = quiz.QuizQuestions.Any(qq => qq.QuestionId == questionId);
            if (alreadyAdded) return await GetByAdminTokenAsync(adminToken);

            int nextPosition = quiz.QuizQuestions.Count == 0 ? 0
                : quiz.QuizQuestions.Max(qq => qq.Position) + 1;

            _db.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = questionId,
                Position = nextPosition
            });

            await _db.SaveChangesAsync();
            return await GetByAdminTokenAsync(adminToken);
        }

        public async Task<QuizDto?> ContributeQuestionAsync(Guid contributorToken, CreateQuestionDto dto)
        {
            var quiz = await _db.Quizzes
                .Where(q => q.ContributorToken == contributorToken)
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync();

            if (quiz is null) return null;

            // Delegate question creation to QuestionService — no logic duplication
            var createdQuestion = await _questionService.CreateAsync(dto);

            int nextPosition = quiz.QuizQuestions.Count == 0
                ? 0
                : quiz.QuizQuestions.Max(qq => qq.Position) + 1;

            _db.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = createdQuestion.Id,
                Position = nextPosition
            });

            await _db.SaveChangesAsync();
            return await GetByContributorTokenAsync(contributorToken);
        }

        /// ==============================================================================
        /// Helpers ----------------------------------------------------------------------
        /// ==============================================================================

        // Single place that defines the standard eager-load shape for a quiz
        private async Task<Quiz?> LoadQuizWithQuestions(
            System.Linq.Expressions.Expression<Func<Quiz, bool>> predicate) =>
            await _db.Quizzes
                .Where(predicate)
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                        .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync();

        private static QuizDto MapToDto(Quiz q)
        {
            return new()
            {
                Id = q.Id,
                Title = q.Title,
                Questions = [.. q.QuizQuestions
                .OrderBy(qq => qq.Position)
                .Select(qq => new QuizQuestionDto
                {
                    Id = qq.Question.Id,
                    Text = qq.Question.Text,
                    CorrectChoiceId = qq.Question.CorrectChoiceId,
                    Choices = [.. qq.Question.Choices
                        .OrderBy(c => c.Position)
                        .Select(c => new ChoiceDto { Id = c.Id, Text = c.Text })]
                })]
            };
        }

        private static QuizCreatedDto MapToCreatedDto(Quiz q)
        {
            return new()
            {
                Id = q.Id,
                Title = q.Title,
                AdminToken = q.AdminToken,
                ContributorToken = q.ContributorToken,
                Questions = [.. q.QuizQuestions
                .OrderBy(qq => qq.Position)
                .Select(qq => new QuizQuestionDto
                {
                    Id = qq.Question.Id,
                    Text = qq.Question.Text,
                    CorrectChoiceId = qq.Question.CorrectChoiceId,
                    Choices = [.. qq.Question.Choices
                        .OrderBy(c => c.Position)
                        .Select(c => new ChoiceDto { Id = c.Id, Text = c.Text })]
                })]
            };
        }
    }
}