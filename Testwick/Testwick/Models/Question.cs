namespace Testwick.Models
{
    public class Question
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Text { get; set; } = string.Empty;
        public int CorrectChoiceId { get; set; }

        // Navigation
        public ICollection<QuestionChoice> Choices { get; set; } = [];
        public ICollection<QuestionTopic> QuestionTopics { get; set; } = [];

        public QuestionChoice? GetCorrectChoice()
        {
            return Choices
                .FirstOrDefault(c => c.Id == CorrectChoiceId);
        }
    }
}
