namespace Testwick.Models
{
    public class QuestionChoice
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Position { get; set; }

        // Navigation
        public Question Question { get; set; } = null!;
    }
}
