using Testwick.DTOs;

namespace Testwick.Services
{
    public interface IQuizResultsService
    {
        // Sumbit answers for a quiz - score is computed server-side
        Task<QuizResultDto> CreateAsync(CreateQuizResultDto dto);
        // Retrieve all results for a given quiz (admin use)
        Task<IEnumerable<QuizResultDto>> GetByQuizIdAsync(int quizId);
        // Retrieve a single result by its ID
        Task<QuizResultDto?> GetByIdAsync(int id);
    }
}
