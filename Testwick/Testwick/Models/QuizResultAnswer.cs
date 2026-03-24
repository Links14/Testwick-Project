namespace Testwick.Models
{
    public class QuizResultAnswer
    {
        public int Id { get; set; }
        public int QuizResultId { get; set; }
        public int QuestionId { get; set; }
        public int? ChosenChoiceId { get; set; }  // nullable = skipped

        // Navigation
        public QuizResult QuizResult { get; set; } = null!;
        public Question Question { get; set; } = null!;
        public QuestionChoice? ChosenChoice { get; set; }
    }
}
