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
        public async Task<QuizAdminDto?> GetByAdminTokenAsync(Guid adminToken)
        {
            var quiz = await LoadQuizWithQuestions(q => q.AdminToken == adminToken);
            return quiz is null ? null : MapToCreatedDto(quiz);
        }
        public async Task<QuizDto?> GetByContributorTokenAsync(Guid contributorToken)
        {
            var quiz = await LoadQuizWithQuestions(q => q.ContributorToken == contributorToken || q.AdminToken == contributorToken); // make admin backwards compatible with lower access contributor
            return quiz is null ? null : MapToDto(quiz);
        }

        // Post
        public async Task<QuizAdminDto> CreateAsync(CreateQuizDto dto)
        {
            var quiz = new Quiz { Title = dto.Title };
            _db.Add(quiz);
            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            _db.QuizQuestions.AddRange(dto.QuestionIds.Select((questionId, i) => new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = questionId,
                Position = i
            }));

            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();

            var created = await LoadQuizWithQuestions(q => q.Id == quiz.Id);
            return MapToCreatedDto(created!);
        }

        public async Task<bool> UpdateByAdminTokenAsync(Guid adminToken, CreateQuizDto dto)
        {
            var quiz = await LoadQuizWithQuestions(q => q.AdminToken == adminToken);
            if (quiz is null) return false;

            quiz.Title = dto.Title;
            // Remove all questions
            var qs = await _db.QuizQuestions.Where(qq => qq.QuizId == quiz.Id).ExecuteDeleteAsync();

            // Add them back
            _db.QuizQuestions.AddRange(dto.QuestionIds.Select((questionId, i) => new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = questionId,
                Position = i
            }));

            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();
            return true;
        }

        public async Task<QuizAdminDto?> AddExistingQuestionByAdminTokenAsync(Guid adminToken, int questionId)
        {
            var quiz = await LoadQuizWithQuestions(q => q.AdminToken == adminToken);

            if (quiz is null) return null;

            var questionExists = await _db.Questions.AnyAsync(q => q.Id == questionId);
            if (!questionExists) return null;

            // check if added to our compared quiz specifically
            bool alreadyAdded = quiz.QuizQuestions
                .Any(qq => qq.QuizId == quiz.Id && qq.QuestionId == questionId);
            if (alreadyAdded) throw new Exception(message: "The question already exists in this quiz!");            

            int nextPosition = quiz.QuizQuestions.Count == 0 ? 0
                : quiz.QuizQuestions.Max(qq => qq.Position) + 1;

            _db.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = questionId,
                Position = nextPosition
            });

            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();
            return await GetByAdminTokenAsync(adminToken);
        }

        public async Task<QuizAdminDto?> RemoveExistingQuestionByAdminTokenAsync(Guid adminToken, int questionId)
        {
            // Get quiz ID only, no need to load navigation collections
            var quizId = await _db.Quizzes
                .Where(q => q.AdminToken == adminToken)
                .Select(q => q.Id)
                .FirstOrDefaultAsync();

            if (quizId == 0) return null;
            
            // Delete the QuizQuestion row directly
            var rowsDeleted = await _db.QuizQuestions
                .Where(qq => qq.QuizId == quizId && qq.QuestionId == questionId)
                .ExecuteDeleteAsync(); // EF Core 7+ method for direct DB deletion

            if (rowsDeleted == 0)
                throw new Exception("No matching question in this quiz to remove.");

            // Clear tracker just in case
            _db.ChangeTracker.Clear();            
            return await GetByAdminTokenAsync(adminToken);
        }


        // does not properly add to db
        public async Task<QuizDto?> ContributeQuestionAsync(Guid contributorToken, CreateQuestionDto dto)
        {
            var quiz = await LoadQuizWithQuestions(
                (q => q.ContributorToken == contributorToken 
                || q.AdminToken == contributorToken));

            if (quiz is null) return null;
            if (dto is null) return null;

            // Delegate question creation to QuestionService — no logic duplication
            var createdQuestion = await _questionService.CreateAsync(dto);
            int id = createdQuestion.Id;

            await _db.SaveChangesAsync();

            int nextPosition = quiz.QuizQuestions.Count == 0 ? 0
                : quiz.QuizQuestions.Max(qq => qq.Position) + 1;
            
            _db.QuizQuestions.Add(new QuizQuestion
            {
                QuizId = quiz.Id,
                QuestionId = id,
                Position = nextPosition
            });

            await _db.SaveChangesAsync();
            _db.ChangeTracker.Clear();
            return await GetByContributorTokenAsync(contributorToken);
        }

        /// ==============================================================================
        /// Helpers ----------------------------------------------------------------------
        /// ==============================================================================

        // Single place that defines the standard eager-load shape for a quiz
        private async Task<Quiz?> LoadQuizWithQuestions(System.Linq.Expressions.Expression<Func<Quiz, bool>> predicate)
        {
            return await _db.Quizzes
                .Where(predicate)
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                        .ThenInclude(q => q.Choices)
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                        .ThenInclude(q => q.QuestionTopics)
                            .ThenInclude(qt => qt.Topic)
                .FirstOrDefaultAsync();
        }

        private static QuizDto MapToDto(Quiz q)
        {
            return new()
            {
                Id = q.Id,
                Title = q.Title,
                Questions = [.. q.QuizQuestions
                .OrderBy(qq => qq.Position)
                .Select(qq => new QuestionDto
                {
                    Id = qq.Question.Id,
                    Text = qq.Question.Text,
                    //CorrectChoiceId = (int)qq.Question.CorrectChoiceId,
                    Choices = [.. qq.Question.Choices
                        .OrderBy(c => c.Position)
                        .Select(c => new ChoiceDto { Id = c.Id, Text = c.Text })],
                    Topics = [.. qq.Question.QuestionTopics
                             .Select(qt => qt.Topic.Name)]
                })]

            };
        }

        private static QuizAdminDto MapToCreatedDto(Quiz q)
        {
            return new()
            {
                Id = q.Id,
                Title = q.Title,
                AdminToken = q.AdminToken,
                ContributorToken = q.ContributorToken,
                Questions = [.. q.QuizQuestions
                .OrderBy(qq => qq.Position)
                .Select(qq => new QuestionAdminDto
                {
                    Id = qq.Question.Id,
                    Text = qq.Question.Text,
                    Choices = [.. qq.Question.Choices
                        .OrderBy(c => c.Position)
                        .Select(c => new ChoiceDto { Id = c.Id, Text = c.Text })],
                    CorrectChoiceId = (int)qq.Question.CorrectChoiceId!,
                    Topics = [.. qq.Question.QuestionTopics
                             .Select(qt => qt.Topic.Name)]                    
                })]
            };
        }
    }
}