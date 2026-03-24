namespace Testwick.Models
{
    public class QuizResult
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public int Score { get; set; }

        // Navigation
        public Quiz Quiz { get; set; } = null!;
        public ICollection<QuizResultAnswer> Answers { get; set; } = [];
    }
}
