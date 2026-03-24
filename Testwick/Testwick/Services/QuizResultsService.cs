using Microsoft.EntityFrameworkCore;
using Testwick.Data;
using Testwick.DTOs;
using Testwick.Models;

namespace Testwick.Services
{
    public class QuizResultsService(AppDbContext db) : IQuizResultsService
    {
        private readonly AppDbContext _db = db;

        public async Task<QuizResultDto> CreateAsync(CreateQuizResultDto dto)
        {
            // Load and validate quiz questions
            var quiz = await _db.Quizzes
                .Where(q => q.Id == dto.QuizId)
                .Include(q => q.QuizQuestions)
                    .ThenInclude(qq => qq.Question)
                        .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync()
                ?? throw new ArgumentException($"Quiz {dto.QuizId} not found.");

            var quizQuestions = quiz.QuizQuestions
                .Select(qq => qq.Question)
                .ToList();

            // Validation
            // Every submitted answer must reference a question in this quiz
            var validQuestionIds = quizQuestions.Select(q => q.Id).ToHashSet();
            var invalidAnswers = dto.Answers
                .Where(a => !validQuestionIds.Contains(a.QuestionId))
                .ToList();

            if (invalidAnswers.Count > 0)
            {
                throw new ArgumentException(
                    $"Answers reference question Ids not in this quiz: " +
                    string.Join(", ", invalidAnswers.Select(a => a.QuestionId)));
            }

            // Validation
            // Each chosen choice must belong to the question its submitted against
            foreach (var answer in dto.Answers.Where(a => a.ChosenChoiceId.HasValue))
            {
                var question = quizQuestions.First(q => q.Id == answer.QuestionId);
                bool choiceIsValid = question.Choices.Any(c => c.Id == answer.ChosenChoiceId);
                if (!choiceIsValid)
                    throw new ArgumentException(
                        $"Choice {answer.ChosenChoiceId} does not belong to question {answer.QuestionId}");
            }

            // Score
            // Count answers where the chosen choice matches the question's correct choice
            int score = dto.Answers.Count(a =>
                a.ChosenChoiceId.HasValue &&
                quizQuestions
                    .First(q => q.Id == a.QuestionId)
                    .CorrectChoiceId == a.ChosenChoiceId);

            var result = new QuizResult
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                QuizId = dto.QuizId,
                Score = score
            };
            _db.QuizResults.Add(result);
            await _db.SaveChangesAsync();

            // Build Answer Records
            // IsCorrect is derived, not stored, but we record the choice
            var answerEntities = dto.Answers.Select(a => new QuizResultAnswer
            {
                QuizResultId = result.Id,
                QuestionId = a.QuestionId,
                ChosenChoiceId = a.ChosenChoiceId
            }).ToList();

            _db.QuizResultAnswers.AddRange(answerEntities);
            await _db.SaveChangesAsync();

            return (await GetByIdAsync(result.Id))!;
        }

        public async Task<IEnumerable<QuizResultDto>> GetByQuizIdAsync(int quizId)
        {
            return await _db.QuizResults
                .Where(r => r.QuizId == quizId)
                .Include(r => r.Answers)
                    .ThenInclude(a => a.Question)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<QuizResultDto?> GetByIdAsync(int id)
        {
            var result = await _db.QuizResults
                .Where(r => r.Id == id)
                .Include(r => r.Answers)
                    .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync();

            return result is null ? null : MapToDto(result);
        }

        private static QuizResultDto MapToDto(QuizResult r)
        {
            return new()
            {
                Id = r.Id,
                FirstName = r.FirstName,
                LastName = r.LastName,
                Score = r.Score,
                QuizId = r.QuizId,
                CreatedAt = r.CreatedAt,
                Answers = [.. r.Answers.Select(a => new QuizResultAnswerDto {
                    QuestionId = a.QuestionId,
                    ChosenChoiceId = a.ChosenChoiceId,
                    // IsCorrect is deried at read time from the question's CorrectChoiceId
                    IsCorrect = a.ChosenChoiceId.HasValue &&
                        a.ChosenChoiceId == a.Question.CorrectChoiceId
                })]
            };
        }
    }
}
