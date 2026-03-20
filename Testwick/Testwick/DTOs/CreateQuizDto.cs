namespace Testwick.DTOs
{
    public class QuizDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<QuizQuestionDto> Questions { get; set; } = [];  // ordered by Position
    }

    public class QuizQuestionDto
    // used inside QuizDto — a lightweight question summary, not the full QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<ChoiceDto> Choices { get; set; } = [];
        public int CorrectChoiceId { get; set; }
    }

    public class CreateQuizDto
    {
        public string Title { get; set; } = string.Empty;
        public List<int> QuestionIds { get; set; } = [];  // ordered, becomes Position
    }
}