using Testwick.DTOs;
using Testwick.Models;
using Testwick.Data;
using Microsoft.EntityFrameworkCore;

namespace Testwick.Services
{
    public class QuestionService(AppDbContext db) : IQuestionService
    {
        private readonly AppDbContext _db = db;

        public Task<Question> CreateAsync(CreateQuestionDto question)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        //public async Task<Question> CreateAsync(CreateQuestionDto question) =>
        //    await _db. ;

        //public async Task<bool> DeleteAsync(int id) =>
        //    await _db. ;
        //    //DeleteAsync(id);

        public async Task<IEnumerable<Question>> GetAllAsync() =>
            await _db.Questions.Include(q => q.Topics).ToListAsync();

        public async Task<Question?> GetByIdAsync(int id) =>
            await _db.Questions.Include(q => q.Topics).FirstOrDefaultAsync(q => q.Id == id);

        public Task<IEnumerable<Question>> GetByTopicAsync(string topic)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(int id, CreateQuestionDto question)
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<Question>> GetByTopicAsync(string topic) =>
        //    await _db. ;

        //public async Task<bool> UpdateAsync(int id, CreateQuestionDto question) =>
        //    await _db. ;
    }
}
