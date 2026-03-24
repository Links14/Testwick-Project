namespace Testwick.Models
{
    public class QuestionTopic
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int TopicId { get; set; }

        // navigation
        public Question Question { get; set; } = null!;
        public Topic Topic { get; set; } = null!;
    }
}
