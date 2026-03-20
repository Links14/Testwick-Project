namespace Testwick.Services
{
    using Models;
    using Testwick.DTOs;

    public interface IQuestionService
    {
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(int id);
        Task<IEnumerable<Question>> GetByTopicAsync(string topic);
        Task<Question> CreateAsync(CreateQuestionDto question);
        Task<bool> UpdateAsync(int id, CreateQuestionDto question);
        Task<bool> DeleteAsync(int id);
    }
}
