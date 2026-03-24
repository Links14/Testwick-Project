namespace Testwick.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid AdminToken { get; set; } = Guid.NewGuid();
        public Guid ContributorToken { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;

        // Navigation
        public ICollection<QuizQuestion> QuizQuestions { get; set; } = [];
        public ICollection<QuizResult> QuizResults { get; set; } = [];
    }
}
