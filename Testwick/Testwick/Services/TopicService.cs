using Microsoft.EntityFrameworkCore;
using Testwick.Data;
using Testwick.DTOs;
using Testwick.Models;

namespace Testwick.Services
{
    public class TopicService(AppDbContext db) : ITopicService
    {
        private readonly AppDbContext _db = db;

        public async Task<IEnumerable<TopicDto>> GetAllAsync()
        {
            return await _db.Topics
                .Select(t => new TopicDto { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        public async Task<TopicDto?> GetByIdAsync(int id)
        {
            var topic = await _db.Topics.FindAsync(id);
            return topic is null ? null : new TopicDto { Id = topic.Id, Name = topic.Name };
        }

        public async Task<TopicDto?> CreateAsync(CreateTopicDto dto)
        {
            if (_db.Topics.Any(t => t.Name == dto.Name))
                return null;

            var topic = new Topic { Name = dto.Name };
            _db.Topics.Add(topic);
            await _db.SaveChangesAsync();
            return new TopicDto { Id = topic.Id, Name = topic.Name };
        }

        //public async Task<bool> UpdateAsync(int id, CreateTopicDto dto)
        //{
        //    var topic = await _db.Topics.FindAsync(id);
        //    if (topic is null) return false;
        //    topic.Name = dto.Name;
        //    await _db.SaveChangesAsync();
        //    return true;
        //}

        public async Task<bool> DeleteAsync(int id)
        {
            var topic = await _db.Topics.FindAsync(id);
            if (topic is null) return false;
            _db.Topics.Remove(topic);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}