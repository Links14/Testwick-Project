namespace Testwick.Models
{
    public class Topic
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Navigation
        public ICollection<QuestionTopic> QuestionTopics { get; set; } = [];
    }
}
