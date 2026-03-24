namespace Testwick.Services
{
    using Testwick.DTOs;

    public interface IQuizService
    {
        // Get - Reads
        Task<IEnumerable<QuizDto>> GetAllAsync();
        Task<QuizDto?> GetByIdAsync(int id);
        Task<QuizDto?> GetByAdminTokenAsync(Guid adminToken);        // admin landing page
        Task<QuizDto?> GetByContributorTokenAsync(Guid contributorToken); // contributor landing page

        // Post
        Task<QuizCreatedDto> CreateAsync(CreateQuizDto dto);
        
        // Put - Admin access Only
        Task<QuizDto?> AddExistingQuestionByAdminTokenAsync(Guid adminToken, int questionId);
        Task<bool> UpdateByAdminTokenAsync(Guid adminToken, CreateQuizDto dto);
        
        // Post - Contributor access only
        Task<QuizDto?> ContributeQuestionAsync(Guid contributorToken, CreateQuestionDto dto);
    }
}
