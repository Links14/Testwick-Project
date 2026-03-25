namespace Testwick.Services
{
    using DTOs;
    public interface ITopicService
    {
        Task<IEnumerable<TopicDto>> GetAllAsync();
        Task<TopicDto?> GetByIdAsync(int id);
        Task<TopicDto> CreateAsync(CreateTopicDto dto);
        //Task<bool> UpdateAsync(int id, CreateTopicDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
