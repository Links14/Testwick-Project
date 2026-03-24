namespace Testwick.Models
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int Position { get; set; }  // this is what preserves order

        // Navigation
        public Question Question { get; set; } = null!;
        public Quiz Quiz { get; set; } = null!;
    }
}