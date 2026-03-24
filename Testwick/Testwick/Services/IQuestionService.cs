namespace Testwick.Services
{
    using Models;
    using Testwick.DTOs;

    public interface IQuestionService
    {
        Task<IEnumerable<QuestionDto>> GetAllAsync();
        Task<QuestionDto?> GetByIdAsync(int id);
        Task<IEnumerable<QuestionDto>> GetByTopicAsync(string topic);
        Task<QuestionDto> CreateAsync(CreateQuestionDto question);
    }
}
