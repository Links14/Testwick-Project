using Testwick.DTOs;
using Testwick.Models;
using Testwick.Data;

namespace Testwick.Services
{
    public class QuizService(AppDbContext db) : IQuizService
    {
        private readonly AppDbContext _db = db;

        public Task<Quiz> CreateAsync(CreateQuizDto dto) =>
            //_db.CreateAsync(dto);
            throw new NotImplementedException();

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Quiz>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Quiz?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, CreateQuizDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
