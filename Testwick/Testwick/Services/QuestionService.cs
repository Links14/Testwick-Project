using Microsoft.EntityFrameworkCore;
using Testwick.Data;
using Testwick.DTOs;
using Testwick.Models;

namespace Testwick.Services
{
    public class QuestionService(AppDbContext db) : IQuestionService
    {
        private readonly AppDbContext _db = db;

        public async Task<IEnumerable<QuestionDto>> GetAllAsync() 
        {
            return await _db.Questions
                .Include(q => q.Choices)
                .Include(q => q.QuestionTopics)
                .ThenInclude(qt => qt.Topic)
                .Select(q => MapToDto(q))
                .ToListAsync();
        }

        public async Task<QuestionDto?> GetByIdAsync(int id)
        {
            var question = await _db.Questions
                .Include(q => q.Choices)
                .Include(q => q.QuestionTopics).ThenInclude(qt => qt.Topic)
                .FirstOrDefaultAsync(q => q.Id == id);

            return question is null ? null : MapToDto(question);
        }
        public async Task<IEnumerable<QuestionDto>> GetByTopicAsync(string topicName)
        {
            return await _db.Questions
                .Include(q => q.Choices)
                .Include(q => q.QuestionTopics).ThenInclude(qt => qt.Topic)
                .Where(q => q.QuestionTopics.Any(qt => qt.Topic.Name == topicName))
                .Select(q => MapToDto(q))
                .ToListAsync();
        }

        public async Task<QuestionDto> CreateAsync(CreateQuestionDto dto)
        {
            if (dto.Choices.Count < 2)
                throw new ArgumentException("A question must have at least 2 choices." +
                    $"\nQuestions has {dto.Choices.Count} choices");

            if (dto.CorrectChoiceIndex < 0 || dto.CorrectChoiceIndex >= dto.Choices.Count)
                throw new ArgumentException("CorrectChoiceIndex must refer to a valid choice.");

            // Create the question first so choices can reference its Id
            var question = new Question
            {
                Text = dto.Text,
                Choices = [.. dto.Choices
                .Select((c, i) =>
                new QuestionChoice() {
                    Text = c.Text,
                    Position = i
                })]
            };

            Console.Write("Add question to db.Questions");
            await _db.Questions.AddAsync(question);
            Console.Write("Saving add question");
            await _db.SaveChangesAsync();
            Console.Write("Saved add question");

            if (question.Choices.Count < dto.CorrectChoiceIndex)
                throw new ArgumentException("CorrectChoiceIndex is greater that the number of choices passed.");
            question.CorrectChoiceId = question.Choices.ToList()[dto.CorrectChoiceIndex].Id;

            // Link requested topics (skip any IDs that don't exist)
            if (dto.TopicIds.Count > 0)
            {
                var existingTopicIds = await _db.Topics
                    .Where(t => dto.TopicIds.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToListAsync();

                _db.QuestionTopics.AddRange(existingTopicIds.Select(topicId => new QuestionTopic
                {
                    QuestionId = question.Id,
                    TopicId = topicId
                }));
            }

            await _db.SaveChangesAsync();
            return (await GetByIdAsync(question.Id))!;
        }

        private static QuestionDto MapToDto(Question q)
        {
            QuestionDto dto = new()
            {
                Id = q.Id,
                Text = q.Text,
                //CorrectChoiceId =q.CorrectChoiceId,
                Choices = [.. q.Choices
                    .OrderBy(c => c.Position)
                    .Select(c => new ChoiceDto { Id = c.Id, Text = c.Text })],
                Topics = [.. q.QuestionTopics.Select(qt => qt.Topic.Name)]
            };

            if (dto.Choices.Count < 2)
                throw new Exception("A question must have at least 2 choices." +
                    $"\nQuestions has {dto.Choices.Count} choices");

            if (q.CorrectChoiceId is null)
                throw new ArgumentException("No valid CorrectChoice Value");

            if (!dto.Choices.Any(c => c.Id == q.CorrectChoiceId))
                throw new Exception("At least one correct answer is required");

            return dto;
        }
    }
}