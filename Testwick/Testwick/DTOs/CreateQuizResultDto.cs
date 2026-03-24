namespace Testwick.DTOs
{
    public class QuizResultDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int QuizId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<QuizResultAnswerDto> Answers { get; set; } = [];
    }

    public class CreateQuizResultDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int QuizId { get; set; }
        public List<CreateQuizResultAnswerDto> Answers { get; set; } = [];
        // Score removed — calculated from Answers, not submitted by the client
    }

}