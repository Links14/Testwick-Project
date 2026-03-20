namespace Testwick.Services
{
    using Models;
    using Testwick.DTOs;

    public interface IQuizService
    {
        Task<IEnumerable<Quiz>> GetAllAsync();
        Task<Quiz?> GetByIdAsync(int id);
        Task<Quiz> CreateAsync(CreateQuizDto dto);
        Task<bool> UpdateAsync(int id, CreateQuizDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
