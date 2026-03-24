namespace Testwick.DTOs
{
    public class QuizResultAnswerDto
    {
        public int QuestionId { get; set; }
        public int? ChosenChoiceId { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class CreateQuizResultAnswerDto
    {
        public int QuestionId { get; set; }
        public int? ChosenChoiceId { get; set; }  // null if skipped
    }
}
