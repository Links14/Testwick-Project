namespace Testwick.Models
{
    public class QuestionChoiceOrder
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public int Position { get; set; }

        // Navigation
        public Question Question { get; set; } = null!;
        public QuestionChoice Choice { get; set; } = null!;
    }
}
