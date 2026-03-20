namespace Testwick.DTOs
{
    public class TopicDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateTopicDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
