namespace Testwick.DTOs
{
    public abstract class QuizBase<IQuestion> where IQuestion : QuestionDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<IQuestion> Questions { get; set; } = [];  // ordered by Position
    }


    // Quiz
    public class QuizDto : QuizBase<QuestionDto> { }

    // Post
    public class CreateQuizDto
    {
        public string Title { get; set; } = string.Empty;
        public List<int> QuestionIds { get; set; } = [];  // ordered, becomes Position
    }


    // Get Admin Context
    public class QuizAdminDto : QuizBase<QuestionAdminDto>
    {
        public Guid AdminToken { get; set; }
        public Guid ContributorToken { get; set; }
    }
}