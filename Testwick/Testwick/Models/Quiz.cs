using System.Collections.ObjectModel;

namespace Testwick.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int CorrectChoiceId { get; set; }

        // Navigation
        public QuestionChoice CorrectChoice { get; set; } = null!;
        public ICollection<QuestionChoiceOrder> QuestionChoiceOrders { get; set; } = [];
        public ICollection<QuestionTopic> QuestionTopics { get; set; } = [];
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
    }
}
