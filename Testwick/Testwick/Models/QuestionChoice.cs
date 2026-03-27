namespace Testwick.Models
{
    public class QuestionChoice
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int QuestionId { get; set; }
        public int Position { get; set; }

        // Navigation
        public Question Question { get; set; } = null!;
    }
}
