namespace Testwick.DTOs
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public List<ChoiceDto> Choices { get; set; } = [];  // ordered by Position
        public int CorrectChoiceId { get; set; }
        public List<string> Topics { get; set; } = [];
    }

    public class CreateQuestionDto
    {
        public string Text { get; set; } = string.Empty;
        public List<CreateChoiceDto> Choices { get; set; } = [];
        public int CorrectChoiceIndex { get; set; }  // index into Choices list above
        public List<int> TopicIds { get; set; } = [];
    }
}